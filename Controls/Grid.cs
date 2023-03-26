using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace FallGuysStats {
    public sealed class Grid : DataGridView {
        public ContextMenuStrip CMenu;
        private IContainer components;
        private SaveFileDialog _saveFile;
        private ToolStripMenuItem ExportItemCsv, ExportItemHtml, ExportItemBbcode, ExportItemMd;
        public ToolStripMenuItem DeleteShows, MoveShows;
        private bool IsEditOnEnter, readOnly;
        private bool? allowUpdate, allowNew, allowDelete;
        public Dictionary<string, SortOrder> Orders = new Dictionary<string, SortOrder>(StringComparer.OrdinalIgnoreCase);
        public Grid() {
            this.SetContextMenu();
            this.AllowUserToAddRows = false;
            this.AllowUserToOrderColumns = true;
            this.AllowUserToResizeRows = false;
            this.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            this.RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.BackgroundColor = Color.FromArgb(234, 242, 251);
            this.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            this.BorderStyle = BorderStyle.None;
            this.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.RowHeadersWidth = 20;
            this.ContextMenuStrip = CMenu;
            this.readOnly = false;
            this.EnableHeadersVisualStyles = false;
            this.ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            this.ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            this.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.Cyan;
            this.ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;
        }
        public SortOrder GetSortOrder(string columnName) {
            this.Orders.TryGetValue(columnName, out SortOrder sortOrder);

            if (sortOrder == SortOrder.None) {
                this.Columns[columnName].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                this.Orders[columnName] = SortOrder.Ascending;
                return SortOrder.Ascending;
            } else if (sortOrder == SortOrder.Ascending) {
                this.Columns[columnName].HeaderCell.SortGlyphDirection = SortOrder.Descending;
                this.Orders[columnName] = SortOrder.Descending;
                return SortOrder.Descending;
            } else {
                this.Columns[columnName].HeaderCell.SortGlyphDirection = SortOrder.None;
                this.Orders[columnName] = SortOrder.None;
                return SortOrder.None;
            }
        }
        public DataGridViewRow CloneWithValues(DataGridViewRow row) {
            DataGridViewRow clonedRow = (DataGridViewRow)row.Clone();
            for (int i = 0; i < row.Cells.Count; i++) {
                clonedRow.Cells[i].Value = row.Cells[i].Value;
                clonedRow.Cells[i].Tag = row.Cells[i].EditedFormattedValue;
            }
            return clonedRow;
        }
        protected override void OnDataSourceChanged(EventArgs e) {
            this.Columns.Clear();
            this.IsEditOnEnter = this.EditMode == DataGridViewEditMode.EditOnEnter;
            base.OnDataSourceChanged(e);
        }
        protected override void OnPaint(PaintEventArgs e) {
            if (this.DesignMode) {
                Graphics g = e.Graphics;
                g.Clear(BackgroundColor);
                Pen bp = new Pen(Color.DarkGray, 1) {
                    DashStyle = System.Drawing.Drawing2D.DashStyle.Solid
                };
                int i = 0;
                g.DrawLine(bp, 0, 0, 0, Height);
                g.DrawLine(bp, Width - 1, 0, Width - 1, Height);
                int header = ((int)ColumnHeadersDefaultCellStyle.Font.Size * 2) + 1;
                g.FillRectangle(new SolidBrush(ColumnHeadersDefaultCellStyle.BackColor), 1, 1, Width - 2, header - 1);
                TextRenderer.DrawText(g, "Header", ColumnHeadersDefaultCellStyle.Font, new Point(10, 2), ColumnHeadersDefaultCellStyle.ForeColor);
                g.FillRectangle(new SolidBrush(DefaultCellStyle.BackColor), 1, header, Width - 2, Height - header - 1);
                g.DrawLine(bp, 0, i, Width, i);
                i += header;
                int row = 1;
                while (i < Height) {
                    g.DrawLine(bp, 0, i, Width, i);
                    TextRenderer.DrawText(g, "Row " + row++, DefaultCellStyle.Font, new Point(10, i + (RowTemplate.Height / 4)), DefaultCellStyle.ForeColor);
                    i += RowTemplate.Height;
                }
                g.DrawLine(bp, 0, Height - 1, Width, Height - 1);
                bp.Dispose();
            } else {
                base.OnPaint(e);
            }
        }
        protected override void OnCellClick(DataGridViewCellEventArgs e) {
            base.OnCellClick(e);
            if (!this.IsEditOnEnter) { return; }
            if (e.ColumnIndex == -1) {
                this.EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
                EndEdit();
            } else if (this.EditMode != DataGridViewEditMode.EditOnEnter) {
                this.EditMode = DataGridViewEditMode.EditOnEnter;
                BeginEdit(false);
            }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(false)]
        public bool Disabled {
            get { return this.readOnly; }
            set {
                this.readOnly = value;
                if (this.readOnly) {
                    this.allowDelete = AllowUserToDeleteRows;
                    this.allowNew = AllowUserToAddRows;
                    this.allowUpdate = ReadOnly;
                    this.AllowUserToAddRows = false;
                    this.AllowUserToDeleteRows = false;
                    this.ReadOnly = true;
                } else if (this.allowNew.HasValue) {
                    this.AllowUserToAddRows = this.allowNew.Value;
                    this.AllowUserToDeleteRows = this.allowDelete.Value;
                    this.ReadOnly = this.allowUpdate.Value;
                }
            }
        }
        protected override void OnLeave(EventArgs e) {
            if (this.IsEditOnEnter) {
                this.EditMode = DataGridViewEditMode.EditOnEnter;
            }
            base.OnLeave(e);
        }
        [DefaultValue(typeof(DataGridViewRowHeadersWidthSizeMode), "DisableResizing")]
        public new DataGridViewRowHeadersWidthSizeMode RowHeadersWidthSizeMode {
            get { return base.RowHeadersWidthSizeMode; }
            set { base.RowHeadersWidthSizeMode = value; }
        }
        [DefaultValue(20)]
        public new int RowHeadersWidth {
            get { return base.RowHeadersWidth; }
            set { base.RowHeadersWidth = value; }
        }
        [DefaultValue(DataGridViewAutoSizeColumnsMode.None)]
        public new DataGridViewAutoSizeColumnsMode AutoSizeColumnsMode {
            get { return base.AutoSizeColumnsMode; }
            set { base.AutoSizeColumnsMode = value; }
        }
        [DefaultValue(false)]
        public new bool AllowUserToAddRows {
            get { return base.AllowUserToAddRows; }
            set { base.AllowUserToAddRows = value; }
        }
        [DefaultValue(true)]
        public new bool AllowUserToOrderColumns {
            get { return base.AllowUserToOrderColumns; }
            set { base.AllowUserToOrderColumns = value; }
        }
        [DefaultValue(false)]
        public new bool AllowUserToResizeRows {
            get { return base.AllowUserToResizeRows; }
            set { base.AllowUserToResizeRows = value; }
        }
        [DefaultValue(DataGridViewEditMode.EditOnKeystrokeOrF2)]
        public new DataGridViewEditMode EditMode {
            get { return base.EditMode; }
            set { base.EditMode = value; }
        }
        [DefaultValue(typeof(Color), "234,242,251")]
        public new Color BackgroundColor {
            get { return base.BackgroundColor; }
            set { base.BackgroundColor = value; }
        }
        public override string ToString() {
            return "Grid(" + Name + ") " + Text;
        }
        public bool HasFocus(Control activeControl) {
            while (activeControl != null) {
                if (activeControl == this) {
                    return true;
                }
                activeControl = activeControl.Parent;
            }
            return false;
        }
        private string EscapeQuotes(string value, char escapeCharacter = ',') {
            return !string.IsNullOrEmpty(value) && value.IndexOf(escapeCharacter) >= 0 ? $"\"{value}\"" : value;
        }

        #region Export event handlers
        private void ExportItemCSV_Click(object sender, EventArgs e) {
            ExportCsv();
        }

        private void ExportItemHTML_Click(object sender, EventArgs e) {
            ExportHtml();
        }

        private void ExportItemBBCODE_Click(object sender, EventArgs e) {
            ExportBbCode();
        }

        private void ExportItemMD_Click(object sender, EventArgs e) {
            ExportMarkdown();
        }
        #endregion

        #region Export methods
        public void ExportCsv() {
            try {
                this._saveFile.Title = Multilingual.GetWord("message_save_csv_file_caption");
                this._saveFile.Filter = "CSV files|*.csv";
                if (this.Name.Equals("gridRoundsSummryList")) {
                    this._saveFile.FileName = $"rounds_summary_list_{DateTime.Now:yyyy-MM-dd hh_mm_ss}";
                } else if (this.Name.Equals("gridShowsStats")) {
                    this._saveFile.FileName = $"shows_stats_list_{DateTime.Now:yyyy-MM-dd hh_mm_ss}";
                } else if (this.Name.Equals("gridRoundsStats")) {
                    this._saveFile.FileName = $"rounds_stats_list_{DateTime.Now:yyyy-MM-dd hh_mm_ss}";
                } else if (this.Name.Equals("gridFinalsStats")) {
                    this._saveFile.FileName = $"finals_stats_list_{DateTime.Now:yyyy-MM-dd hh_mm_ss}";
                } else if (this.Name.Equals("gridRoundStats")) {
                    this._saveFile.FileName = $"round_stats_list_{DateTime.Now:yyyy-MM-dd hh_mm_ss}";
                }

                if (this._saveFile.ShowDialog() == DialogResult.OK) {
                    Encoding enc = Encoding.GetEncoding("utf-8");
                    using (FileStream fs = new FileStream(this._saveFile.FileName, FileMode.Create)) {
                        List<DataGridViewColumn> columns = this.GetSortedColumns();

                        StringBuilder sb = new StringBuilder();
                        foreach (DataGridViewColumn col in columns) {
                            string header = string.IsNullOrEmpty(col.HeaderText) ? col.ToolTipText : col.HeaderText;
                            sb.Append(EscapeQuotes(header)).Append(",");
                        }
                        if (sb.Length > 0) { sb.Length--; }

                        sb.AppendLine();
                        byte[] bytes = enc.GetBytes(sb.ToString());
                        fs.Write(bytes, 0, bytes.Length);

                        foreach (DataGridViewRow row in this.Rows) {
                            sb.Length = 0;
                            foreach (DataGridViewColumn col in columns) {
                                string formattedValue = row.Cells[col.Name].FormattedValue?.ToString();
                                string tooltip = row.Cells[col.Name].ToolTipText;

                                if (string.IsNullOrEmpty(tooltip) || row.Cells[col.Name].FormattedValueType == typeof(string)) {
                                    sb.Append($"{EscapeQuotes(formattedValue)},");
                                } else {
                                    sb.Append($"{EscapeQuotes(tooltip)},");
                                }
                            }
                            if (sb.Length > 0) { sb.Length--; }

                            sb.AppendLine();
                            bytes = enc.GetBytes(sb.ToString());
                            fs.Write(bytes, 0, bytes.Length);
                        }
                        fs.Flush();
                        fs.Close();
                    }

                    MessageBox.Show(this, $"{Multilingual.GetWord("message_save_csv")}{Environment.NewLine}({this._saveFile.FileName})", Multilingual.GetWord("message_save_csv_caption"), MessageBoxButtons.OK);
                }
            } catch (Exception ex) {
                ControlErrors.HandleException(this, ex, false);
            }
        }

        public void ExportHtml() {
            try {
                List<DataGridViewColumn> columns = this.GetSortedColumns();

                StringBuilder sb = new StringBuilder();
                sb.Append("<table><tr>");
                foreach (DataGridViewColumn col in columns) {
                    string header = string.IsNullOrEmpty(col.HeaderText) ? col.ToolTipText : col.HeaderText;
                    sb.Append($"<td><b>{header}</b></td>");
                }
                sb.AppendLine("</tr>");

                foreach (DataGridViewRow row in this.Rows) {
                    sb.Append("<tr>");
                    foreach (DataGridViewColumn col in columns) {
                        string formattedValue = row.Cells[col.Name].FormattedValue?.ToString();
                        string tooltip = row.Cells[col.Name].ToolTipText;

                        if (string.IsNullOrEmpty(tooltip) || row.Cells[col.Name].FormattedValueType == typeof(string)) {
                            sb.Append($"<td>{formattedValue}</td>");
                        } else {
                            sb.Append($"<td>{tooltip}</td>");
                        }
                    }
                    sb.AppendLine("</tr>");
                }
                sb.Append("</table>");
                Clipboard.SetText(sb.ToString(), TextDataFormat.Text);

                MessageBox.Show(this, Multilingual.GetWord("message_save_html"), Multilingual.GetWord("message_save_html_caption"), MessageBoxButtons.OK);
            } catch (Exception ex) {
                ControlErrors.HandleException(this, ex, false);
            }
        }

        public void ExportBbCode() {
            try {
                List<DataGridViewColumn> columns = this.GetSortedColumns();

                StringBuilder sb = new StringBuilder();
                sb.Append("[table][tr]");
                foreach (DataGridViewColumn col in columns) {
                    string header = string.IsNullOrEmpty(col.HeaderText) ? col.ToolTipText : col.HeaderText;
                    sb.Append($"[th]{header}[/th]");
                }
                sb.Append("[/tr]");

                foreach (DataGridViewRow row in this.Rows) {
                    sb.Append("[tr]");
                    foreach (DataGridViewColumn col in columns) {
                        string formattedValue = row.Cells[col.Name].FormattedValue?.ToString();
                        string tooltip = row.Cells[col.Name].ToolTipText;

                        if (string.IsNullOrEmpty(tooltip) || row.Cells[col.Name].FormattedValueType == typeof(string)) {
                            sb.Append($"[td]{formattedValue}[/td]");
                        } else {
                            sb.Append($"[td]{tooltip}[/td]");
                        }
                    }
                    sb.Append("[/tr]");
                }
                sb.Append("[/table]");
                Clipboard.SetText(sb.ToString(), TextDataFormat.Text);

                MessageBox.Show(this, Multilingual.GetWord("message_save_bbcode"), Multilingual.GetWord("message_save_bbcode_caption"), MessageBoxButtons.OK);
            } catch (Exception ex) {
                ControlErrors.HandleException(this, ex, false);
            }
        }

        public void ExportMarkdown() {
            try {
                List<DataGridViewColumn> columns = this.GetSortedColumns();

                StringBuilder sb = new StringBuilder();
                foreach (DataGridViewColumn col in columns) {
                    string header = string.IsNullOrEmpty(col.HeaderText) ? col.ToolTipText : col.HeaderText;
                    sb.Append($"|{header}");
                }
                sb.AppendLine();
                foreach (DataGridViewColumn col in columns) {
                    sb.Append($"|---");
                }
                sb.AppendLine();

                foreach (DataGridViewRow row in this.Rows) {
                    foreach (DataGridViewColumn col in columns) {
                        string formattedValue = row.Cells[col.Name].FormattedValue?.ToString();
                        string tooltip = row.Cells[col.Name].ToolTipText;

                        if (string.IsNullOrEmpty(tooltip) || row.Cells[col.Name].FormattedValueType == typeof(string)) {
                            sb.Append($"|{formattedValue}");
                        } else {
                            sb.Append($"|{tooltip}");
                        }
                    }
                    sb.AppendLine();
                }

                Clipboard.SetText(sb.ToString(), TextDataFormat.Text);

                MessageBox.Show(this, Multilingual.GetWord("message_save_markdown"), Multilingual.GetWord("message_save_markdown_caption"), MessageBoxButtons.OK);
            } catch (Exception ex) {
                ControlErrors.HandleException(this, ex, false);
            }
        }
        #endregion

        private List<DataGridViewColumn> GetSortedColumns() {
            List<DataGridViewColumn> columns = new List<DataGridViewColumn>();
            foreach (DataGridViewColumn col in this.Columns) {
                if (!col.Visible || (string.IsNullOrEmpty(col.HeaderText) && string.IsNullOrEmpty(col.ToolTipText))) { continue; }

                columns.Add(col);
            }
            columns.Sort(delegate (DataGridViewColumn one, DataGridViewColumn two) {
                return one.DisplayIndex.CompareTo(two.DisplayIndex);
            });
            return columns;
        }
        private void Grid_DataError(object sender, DataGridViewDataErrorEventArgs e) {
            e.ThrowException = false;
        }
        protected override bool ProcessDialogKey(Keys keyData) {
            /*if (ProcessKeyStroke(keyData)) {
                return true;
            }*/

            return base.ProcessDialogKey(keyData);
        }
        protected override bool ProcessDataGridViewKey(KeyEventArgs e) {
            /*if (ProcessKeyStroke(e.KeyData)) {
                return true;
            }*/

            return base.ProcessDataGridViewKey(e);
        }
        private bool ProcessKeyStroke(Keys keyData) {
            if (keyData == Keys.Tab) {
                int iCol = this.CurrentCell.ColumnIndex + 1;
                while (iCol < Columns.Count) {
                    DataGridViewColumn col = Columns[iCol];
                    if (!col.ReadOnly && col.Visible) {
                        break;
                    }
                    iCol++;
                }

                if (iCol < Columns.Count) {
                    this.CurrentCell = Rows[this.CurrentCell.RowIndex].Cells[iCol];
                } else {
                    for (iCol = 0; iCol <= this.CurrentCell.ColumnIndex; iCol++) {
                        DataGridViewColumn col = Columns[iCol];
                        if (!col.ReadOnly && col.Visible) {
                            break;
                        }
                    }

                    if (iCol <= this.CurrentCell.ColumnIndex) {
                        this.CurrentCell = this.CurrentCell.RowIndex + 1 < Rows.Count ? Rows[this.CurrentCell.RowIndex + 1].Cells[iCol] : Rows[0].Cells[iCol];
                    }
                }
                return true;
            } else if ((keyData & Keys.Shift) != 0 && (keyData & Keys.Tab) != 0) {
                int iCol = this.CurrentCell.ColumnIndex - 1;
                while (iCol >= 0) {
                    DataGridViewColumn col = Columns[iCol];
                    if (!col.ReadOnly && col.Visible) {
                        break;
                    }
                    iCol--;
                }

                if (iCol >= 0) {
                    this.CurrentCell = Rows[this.CurrentCell.RowIndex].Cells[iCol];
                } else {
                    for (iCol = Columns.Count - 1; iCol >= this.CurrentCell.ColumnIndex; iCol--) {
                        DataGridViewColumn col = Columns[iCol];
                        if (!col.ReadOnly && col.Visible) {
                            break;
                        }
                    }

                    if (iCol >= this.CurrentCell.ColumnIndex) {
                        this.CurrentCell = this.CurrentCell.RowIndex - 1 >= 0 ? Rows[this.CurrentCell.RowIndex - 1].Cells[iCol] : Rows[0].Cells[iCol];
                    }
                }
                return true;
            }
            return false;
        }
        public void Setup(string column, int index, int width = -1, string header = null, DataGridViewContentAlignment align = DataGridViewContentAlignment.MiddleCenter) {
            if (this.Columns == null || this.Columns[column] == null) { return; }
            this.Columns[column].Visible = true;
            this.Columns[column].DisplayIndex = index;
            this.Columns[column].SortMode = DataGridViewColumnSortMode.Automatic;
            this.Columns[column].DefaultCellStyle.Alignment = align;
            if (width > 0) {
                this.Columns[column].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                this.Columns[column].Width = width;
            } else if (width == 0) {
                this.Columns[column].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            } else if (width < 0) {
                this.Columns[column].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            this.Columns[column].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            if (header != null) {
                this.Columns[column].HeaderText = header;
            }
        }
        public void DeallocContextMenu() {
            this.ContextMenuStrip = null;
        }
        private void SetContextMenu() {
            this.components = new Container();
            //ComponentResourceManager resources = new ComponentResourceManager(typeof(Grid));
            this.CMenu = new ContextMenuStrip(this.components);
            this.ExportItemCsv = new ToolStripMenuItem();
            this.ExportItemHtml = new ToolStripMenuItem();
            this.ExportItemBbcode = new ToolStripMenuItem();
            this.ExportItemMd = new ToolStripMenuItem();
            this._saveFile = new SaveFileDialog();
            this.CMenu.SuspendLayout();
            ((ISupportInitialize)this).BeginInit();
            this.SuspendLayout();
            // 
            // CMenu
            // 
            this.CMenu.Items.AddRange(new ToolStripItem[] { this.ExportItemCsv, this.ExportItemHtml, this.ExportItemBbcode, this.ExportItemMd });
            this.CMenu.Name = "contextMenu";
            this.CMenu.Size = new Size(135, 48);
            // 
            // ExportItemCSV
            // 
            this.ExportItemCsv.Name = "exportItemCSV";
            this.ExportItemCsv.Size = new Size(134, 22);
            this.ExportItemCsv.Text = Multilingual.GetWord("main_export_csv");
            this.ExportItemCsv.ShowShortcutKeys = true;
            this.ExportItemCsv.Image = Properties.Resources.export;
            this.ExportItemCsv.ShortcutKeys = Keys.Control | Keys.S;
            this.ExportItemCsv.Click += new EventHandler(this.ExportItemCSV_Click);
            // 
            // ExportItemHTML
            // 
            this.ExportItemHtml.Name = "exportItemHTML";
            this.ExportItemHtml.Size = new Size(134, 22);
            this.ExportItemHtml.Text = Multilingual.GetWord("main_export_html");
            this.ExportItemHtml.ShowShortcutKeys = true;
            this.ExportItemHtml.Image = Properties.Resources.export;
            this.ExportItemHtml.ShortcutKeys = Keys.Control | Keys.E;
            this.ExportItemHtml.Click += new EventHandler(this.ExportItemHTML_Click);
            // 
            // ExportItemBBCODE
            // 
            this.ExportItemBbcode.Name = "exportItemBBCODE";
            this.ExportItemBbcode.Size = new Size(134, 22);
            this.ExportItemBbcode.Text = Multilingual.GetWord("main_export_bbcode");
            this.ExportItemBbcode.ShowShortcutKeys = true;
            this.ExportItemBbcode.Image = Properties.Resources.export;
            this.ExportItemBbcode.ShortcutKeys = Keys.Control | Keys.B;
            this.ExportItemBbcode.Click += new EventHandler(this.ExportItemBBCODE_Click);
            // 
            // ExportItemMD
            // 
            this.ExportItemMd.Name = "exportItemMD";
            this.ExportItemMd.Size = new Size(134, 22);
            this.ExportItemMd.Text = Multilingual.GetWord("main_export_markdown");
            this.ExportItemMd.ShowShortcutKeys = true;
            this.ExportItemMd.Image = Properties.Resources.export;
            this.ExportItemMd.ShortcutKeys = Keys.Control | Keys.M;
            this.ExportItemMd.Click += new EventHandler(this.ExportItemMD_Click);
            // 
            // saveFile
            // 
            this._saveFile.Filter = "CSV files|*.csv";
            this._saveFile.Title = "Save as CSV file";
            // 
            // Grid
            // 
            this.DataError += new DataGridViewDataErrorEventHandler(this.Grid_DataError);
            this.CMenu.ResumeLayout(false);
            ((ISupportInitialize)this).EndInit();
            this.ResumeLayout(false);
        }
        public static DataTable Convert(IEnumerable array, params string[] columns) {
            object rec = null;
            foreach (object o in array) { rec = o; break; }
            DataTable dt = new DataTable();
            if (rec == null) { return dt; }
            PropertyInfo[] properties = rec.GetType().GetProperties();
            dt.Columns.Add(".", rec.GetType());
            foreach (PropertyInfo pi in properties) {
                Type type = pi.PropertyType;
                if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                    type = pi.PropertyType.GetGenericArguments()[0];
                }
                bool addColumn = true;
                if (columns != null && columns.Length > 0) {
                    bool found = false;
                    for (int i = columns.Length - 1; i >= 0; i--) {
                        if (pi.Name.Equals(columns[i], StringComparison.OrdinalIgnoreCase)) {
                            found = true;
                            break;
                        }
                    }
                    addColumn = found;
                }
                if (addColumn) {
                    dt.Columns.Add(pi.Name, type);
                }
            }
            foreach (object o in array) {
                DataRow dr = dt.NewRow();
                dr["."] = o;
                foreach (PropertyInfo pi in properties) {
                    if (columns == null || columns.Length == 0 || dt.Columns.Contains(pi.Name)) {
                        dr[pi.Name] = pi.GetValue(o, null) ?? DBNull.Value;
                    }
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public void ChangeContextMenuLanguage() {
            this.ExportItemCsv.Text = Multilingual.GetWord("main_export_csv");
            this.ExportItemHtml.Text = Multilingual.GetWord("main_export_html");
            this.ExportItemBbcode.Text = Multilingual.GetWord("main_export_bbcode");
            this.ExportItemMd.Text = Multilingual.GetWord("main_export_markdown");
        }
    }
    public static class ControlErrors {
        public static event Action<object, Exception> Error;
        internal static Exception HandleException(object sender, Exception ex, bool rethrow = true) {
            if (!ex.Data.Contains("Handled"))
                ex.Data.Add("Handled", false);
            if (!ex.Data.Contains("Thread"))
                ex.Data.Add("Thread", Thread.CurrentThread.Name);
            if (string.IsNullOrEmpty(ex.Message))
                ex.Data["Handled"] = true;
            if (rethrow)
                throw ex;

            if (!(bool)ex.Data["Handled"]) {
                if (Error != null) {
                    Error(sender, ex);
                } else {
                    MessageBox.Show(ex.Message);
                }
                ex.Data["Handled"] = true;
            }
            return ex;
        }
    }
}