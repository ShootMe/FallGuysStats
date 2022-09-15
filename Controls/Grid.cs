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
        private ContextMenuStrip cMenu;
        private IContainer components;
        private SaveFileDialog saveFile;
        private ToolStripMenuItem exportItemCSV, exportItemHTML, exportItemBBCODE, exportItemMD;
        private bool IsEditOnEnter, readOnly;
        private bool? allowUpdate, allowNew, allowDelete;
        public Dictionary<string, SortOrder> Orders = new Dictionary<string, SortOrder>(StringComparer.OrdinalIgnoreCase);
        public Grid() {
            InitializeComponent();
            AllowUserToAddRows = false;
            AllowUserToOrderColumns = true;
            AllowUserToResizeRows = false;
            EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
            RowHeadersWidthSizeMode = DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            BackgroundColor = Color.FromArgb(234, 242, 251);
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            BorderStyle = BorderStyle.None;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            RowHeadersWidth = 20;
            ContextMenuStrip = cMenu;
            readOnly = false;
            EnableHeadersVisualStyles = false;
            ColumnHeadersDefaultCellStyle.BackColor = Color.LightGray;
            ColumnHeadersDefaultCellStyle.ForeColor = Color.Black;
            ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.Cyan;
            ColumnHeadersDefaultCellStyle.SelectionForeColor = Color.Black;
        }
        public SortOrder GetSortOrder(string columnName) {
            SortOrder sortOrder;
            Orders.TryGetValue(columnName, out sortOrder);

            if (sortOrder == SortOrder.None) {
                Columns[columnName].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                Orders[columnName] = SortOrder.Ascending;
                return SortOrder.Ascending;
            } else if(sortOrder == SortOrder.Ascending){
                Columns[columnName].HeaderCell.SortGlyphDirection = SortOrder.Descending;
                Orders[columnName] = SortOrder.Descending;
                return SortOrder.Descending;
            } else {
                Columns[columnName].HeaderCell.SortGlyphDirection = SortOrder.None;
                Orders[columnName] = SortOrder.None;
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
            Columns.Clear();
            IsEditOnEnter = EditMode == DataGridViewEditMode.EditOnEnter;
            base.OnDataSourceChanged(e);
        }
        protected override void OnPaint(PaintEventArgs e) {
            if (DesignMode) {
                Graphics g = e.Graphics;
                g.Clear(BackgroundColor);
                Pen bp = new Pen(Color.DarkGray, 1);
                bp.DashStyle = System.Drawing.Drawing2D.DashStyle.Solid;
                int i = 0;
                g.DrawLine(bp, 0, 0, 0, Height);
                g.DrawLine(bp, Width - 1, 0, Width - 1, Height);
                int header = (int)ColumnHeadersDefaultCellStyle.Font.Size * 2 + 1;
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
            if (!IsEditOnEnter) { return; }
            if (e.ColumnIndex == -1) {
                EditMode = DataGridViewEditMode.EditOnKeystrokeOrF2;
                EndEdit();
            } else if (EditMode != DataGridViewEditMode.EditOnEnter) {
                EditMode = DataGridViewEditMode.EditOnEnter;
                BeginEdit(false);
            }
        }
        [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden), DefaultValue(false)]
        public bool Disabled {
            get { return readOnly; }
            set {
                readOnly = value;
                if (readOnly) {
                    allowDelete = AllowUserToDeleteRows;
                    allowNew = AllowUserToAddRows;
                    allowUpdate = ReadOnly;
                    AllowUserToAddRows = false;
                    AllowUserToDeleteRows = false;
                    ReadOnly = true;
                } else if (allowNew.HasValue) {
                    AllowUserToAddRows = allowNew.Value;
                    AllowUserToDeleteRows = allowDelete.Value;
                    ReadOnly = allowUpdate.Value;
                }
            }
        }
        protected override void OnLeave(EventArgs e) {
            if (IsEditOnEnter) {
                EditMode = DataGridViewEditMode.EditOnEnter;
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
            if (!string.IsNullOrEmpty(value) && value.IndexOf(escapeCharacter) >= 0) {
                return $"\"{value}\"";
            }
            return value;
        }

        #region Export event handlers
        private void exportItemCSV_Click(object sender, EventArgs e) {
            ExportCsv();
        }

        private void exportItemHTML_Click(object sender, EventArgs e) {
            ExportHtml();
        }

        private void exportItemBBCODE_Click(object sender, EventArgs e) {
            ExportBbCode();
        }

        private void exportItemMD_Click(object sender, EventArgs e) {
            ExportMarkdown();
        }
        #endregion

        #region Export methods
        public void ExportCsv() {
            try {
                saveFile.Filter = "CSV files|*.csv";
                if (saveFile.ShowDialog() == DialogResult.OK) {
                    Encoding enc = Encoding.GetEncoding("windows-1252");
                    using (FileStream fs = new FileStream(saveFile.FileName, FileMode.Create)) {
                        List<DataGridViewColumn> columns = GetSortedColumns();

                        StringBuilder sb = new StringBuilder();
                        foreach (DataGridViewColumn col in columns) {
                            string header = string.IsNullOrEmpty(col.HeaderText) ? col.ToolTipText : col.HeaderText;
                            sb.Append(EscapeQuotes(header)).Append(",");
                        }
                        if (sb.Length > 0) { sb.Length = sb.Length - 1; }

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
                            if (sb.Length > 0) { sb.Length = sb.Length - 1; }

                            sb.AppendLine();
                            bytes = enc.GetBytes(sb.ToString());
                            fs.Write(bytes, 0, bytes.Length);
                        }
                        fs.Flush();
                        fs.Close();
                    }

                    MessageBox.Show(this, $"Saved CSV to {saveFile.FileName}", "Export", MessageBoxButtons.OK);
                }
            } catch (Exception ex) {
                ControlErrors.HandleException(this, ex, false);
            }
        }

        public void ExportHtml() {
            try {
                List<DataGridViewColumn> columns = GetSortedColumns();

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

                MessageBox.Show(this, "Saved Html to clipboard.", "Export", MessageBoxButtons.OK);
            } catch (Exception ex) {
                ControlErrors.HandleException(this, ex, false);
            }
        }

        public void ExportBbCode() {
            try {
                List<DataGridViewColumn> columns = GetSortedColumns();

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

                MessageBox.Show(this, "Saved BBCode to clipboard.", "Export", MessageBoxButtons.OK);
            } catch (Exception ex) {
                ControlErrors.HandleException(this, ex, false);
            }
        }

        public void ExportMarkdown() {
            try {
                List<DataGridViewColumn> columns = GetSortedColumns();

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

                MessageBox.Show(this, "Saved MarkDown to clipboard.", "Export", MessageBoxButtons.OK);
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
            if (ProcessKeyStroke(keyData)) {
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }
        protected override bool ProcessDataGridViewKey(KeyEventArgs e) {
            if (ProcessKeyStroke(e.KeyData)) {
                return true;
            }

            return base.ProcessDataGridViewKey(e);
        }
        private bool ProcessKeyStroke(Keys keyData) {
            if (keyData == Keys.Tab) {
                int iCol = CurrentCell.ColumnIndex + 1;
                while (iCol < Columns.Count) {
                    DataGridViewColumn col = Columns[iCol];
                    if (!col.ReadOnly && col.Visible) {
                        break;
                    }
                    iCol++;
                }

                if (iCol < Columns.Count) {
                    CurrentCell = Rows[CurrentCell.RowIndex].Cells[iCol];
                } else {
                    for (iCol = 0; iCol <= CurrentCell.ColumnIndex; iCol++) {
                        DataGridViewColumn col = Columns[iCol];
                        if (!col.ReadOnly && col.Visible) {
                            break;
                        }
                    }

                    if (iCol <= CurrentCell.ColumnIndex) {
                        if (CurrentCell.RowIndex + 1 < Rows.Count) {
                            CurrentCell = Rows[CurrentCell.RowIndex + 1].Cells[iCol];
                        } else {
                            CurrentCell = Rows[0].Cells[iCol];
                        }
                    }
                }
                return true;
            } else if ((keyData & Keys.Shift) != 0 && (keyData & Keys.Tab) != 0) {
                int iCol = CurrentCell.ColumnIndex - 1;
                while (iCol >= 0) {
                    DataGridViewColumn col = Columns[iCol];
                    if (!col.ReadOnly && col.Visible) {
                        break;
                    }
                    iCol--;
                }

                if (iCol >= 0) {
                    CurrentCell = Rows[CurrentCell.RowIndex].Cells[iCol];
                } else {
                    for (iCol = Columns.Count - 1; iCol >= CurrentCell.ColumnIndex; iCol--) {
                        DataGridViewColumn col = Columns[iCol];
                        if (!col.ReadOnly && col.Visible) {
                            break;
                        }
                    }

                    if (iCol >= CurrentCell.ColumnIndex) {
                        if (CurrentCell.RowIndex - 1 >= 0) {
                            CurrentCell = Rows[CurrentCell.RowIndex - 1].Cells[iCol];
                        } else {
                            CurrentCell = Rows[0].Cells[iCol];
                        }
                    }
                }
                return true;
            }
            return false;
        }
        public void Setup(string column, int index, int width = -1, string header = null, DataGridViewContentAlignment align = DataGridViewContentAlignment.MiddleCenter) {
            if (Columns == null || Columns[column] == null) { return; }
            Columns[column].Visible = true;
            Columns[column].DisplayIndex = index;
            Columns[column].SortMode = DataGridViewColumnSortMode.Automatic;
            Columns[column].DefaultCellStyle.Alignment = align;
            if (width > 0) {
                Columns[column].AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
                Columns[column].Width = width;
            } else if (width == 0) {
                Columns[column].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            } else if (width < 0) {
                Columns[column].AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
            }
            Columns[column].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            if (header != null) {
                Columns[column].HeaderText = header;
            }
        }
        private void InitializeComponent() {
            this.components = new Container();
            ComponentResourceManager resources = new ComponentResourceManager(typeof(Grid));
            this.cMenu = new ContextMenuStrip(this.components);
            this.exportItemCSV = new ToolStripMenuItem();
            this.exportItemHTML = new ToolStripMenuItem();
            this.exportItemBBCODE = new ToolStripMenuItem();
            this.exportItemMD = new ToolStripMenuItem();
            this.saveFile = new SaveFileDialog();
            this.cMenu.SuspendLayout();
            ((ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // cMenu
            // 
            this.cMenu.Items.AddRange(new ToolStripItem[] { this.exportItemCSV, this.exportItemHTML, this.exportItemBBCODE, this.exportItemMD });
            this.cMenu.Name = "contextMenu";
            this.cMenu.Size = new Size(135, 48);
            // 
            // exportItemCSV
            // 
            this.exportItemCSV.Name = "exportItemCSV";
            this.exportItemCSV.Size = new Size(134, 22);
            this.exportItemCSV.Text = "Export CSV";
            this.exportItemCSV.ShowShortcutKeys = true;
            this.exportItemCSV.Image = Properties.Resources.export;
            this.exportItemCSV.ShortcutKeys = Keys.Control | Keys.S;
            this.exportItemCSV.Click += new EventHandler(this.exportItemCSV_Click);
            // 
            // exportItemHTML
            // 
            this.exportItemHTML.Name = "exportItemHTML";
            this.exportItemHTML.Size = new Size(134, 22);
            this.exportItemHTML.Text = "Export HTML";
            this.exportItemHTML.ShowShortcutKeys = true;
            this.exportItemHTML.Image = Properties.Resources.export;
            this.exportItemHTML.ShortcutKeys = Keys.Control | Keys.E;
            this.exportItemHTML.Click += new EventHandler(this.exportItemHTML_Click);
            // 
            // exportItemBBCODE
            // 
            this.exportItemBBCODE.Name = "exportItemBBCODE";
            this.exportItemBBCODE.Size = new Size(134, 22);
            this.exportItemBBCODE.Text = "Export BBCode";
            this.exportItemBBCODE.ShowShortcutKeys = true;
            this.exportItemBBCODE.Image = Properties.Resources.export;
            this.exportItemBBCODE.ShortcutKeys = Keys.Control | Keys.B;
            this.exportItemBBCODE.Click += new EventHandler(this.exportItemBBCODE_Click);
            // 
            // exportItemMD
            // 
            this.exportItemMD.Name = "exportItemMD";
            this.exportItemMD.Size = new Size(134, 22);
            this.exportItemMD.Text = "Export MarkDown";
            this.exportItemMD.ShowShortcutKeys = true;
            this.exportItemMD.Image = Properties.Resources.export;
            this.exportItemMD.ShortcutKeys = Keys.Control | Keys.M;
            this.exportItemMD.Click += new EventHandler(this.exportItemMD_Click);
            // 
            // saveFile
            // 
            this.saveFile.Filter = "CSV files|*.csv";
            this.saveFile.Title = "Save Results";
            // 
            // Grid
            // 
            this.DataError += new DataGridViewDataErrorEventHandler(this.Grid_DataError);
            this.cMenu.ResumeLayout(false);
            ((ISupportInitialize)(this)).EndInit();
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