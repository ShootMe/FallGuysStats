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
        private ToolStripMenuItem exportItem;
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
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells;
            BorderStyle = BorderStyle.None;
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
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
            SortOrder sortOrder = SortOrder.None;
            Orders.TryGetValue(columnName, out sortOrder);

            if (sortOrder == SortOrder.None || sortOrder == SortOrder.Descending) {
                Columns[columnName].HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                Orders[columnName] = SortOrder.Ascending;
                return SortOrder.Ascending;
            } else {
                Columns[columnName].HeaderCell.SortGlyphDirection = SortOrder.Descending;
                Orders[columnName] = SortOrder.Descending;
                return SortOrder.Descending;
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
        [DefaultValue(DataGridViewAutoSizeColumnsMode.AllCells)]
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
        private void exportItem_Click(object sender, EventArgs e) {
            try {
                saveFile.Filter = "CSV files|*.csv";
                if (saveFile.ShowDialog() == DialogResult.OK) {
                    Encoding enc = Encoding.GetEncoding("windows-1252");
                    using (FileStream fs = new FileStream(saveFile.FileName, FileMode.Create)) {
                        StringBuilder sb = new StringBuilder();
                        foreach (DataGridViewColumn col in this.Columns) {
                            if (col.ValueType != null && col.Visible) {
                                sb.Append(col.Name).Append(",");
                            }
                        }
                        if (sb.Length > 0) { sb.Length = sb.Length - 1; }
                        sb.AppendLine();
                        byte[] bytes = enc.GetBytes(sb.ToString());
                        fs.Write(bytes, 0, bytes.Length);
                        foreach (DataGridViewRow row in this.Rows) {
                            sb.Length = 0;
                            foreach (DataGridViewColumn col in this.Columns) {
                                if (!col.Visible) { continue; }
                                if (col.ValueType == typeof(string)) {
                                    sb.Append("\"").Append(row.Cells[col.Name].Value.ToString()).Append("\",");
                                } else if (col.ValueType != null) {
                                    if (row.Cells[col.Name].Value == null) {
                                        sb.Append(",");
                                    } else {
                                        sb.Append(row.Cells[col.Name].Value.ToString()).Append(",");
                                    }
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
                }
            } catch (Exception ex) {
                ControlErrors.HandleException(this, ex, false);
            }
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
            this.exportItem = new ToolStripMenuItem();
            this.saveFile = new SaveFileDialog();
            this.cMenu.SuspendLayout();
            ((ISupportInitialize)(this)).BeginInit();
            this.SuspendLayout();
            // 
            // cMenu
            // 
            this.cMenu.Items.AddRange(new ToolStripItem[] { this.exportItem });
            this.cMenu.Name = "contextMenu";
            this.cMenu.Size = new Size(135, 48);
            // 
            // exportItem
            // 
            this.exportItem.Name = "exportItem";
            this.exportItem.Size = new Size(134, 22);
            this.exportItem.Text = "&Export to CSV";
            this.exportItem.ShowShortcutKeys = true;
            this.exportItem.ShortcutKeys = Keys.Control | Keys.S;
            this.exportItem.Click += new EventHandler(this.exportItem_Click);
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