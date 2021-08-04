using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace invoiceSys
{
    public partial class forminvoice : Form
    {
        public forminvoice()
        {
            InitializeComponent();
        }

        

        private void label2_Click_1(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://dewansoft.com/");
        }

        private void forminvoice_Load(object sender, EventArgs e)
        {
            textDate.Text = DateTime.Now.ToString("yyyy/MM/dd");
            Dictionary<int, string> itemsData = new Dictionary<int, string>();
            itemsData.Add(19000,"لاب توب Dell");
            itemsData.Add(18000, "لاب توب hp");
            itemsData.Add(14000, "لاب توب سامسونج");
            itemsData.Add(20, "كيبورد عادي");
            itemsData.Add(15, "ماوس عادي");
            itemsData.Add(70, "كيبورد hp");
            itemsData.Add(50, "ماوس hp");
            itemsData.Add(1000, "طابعه حبر Dell");
            itemsData.Add(2000, "طابعه ليزر Dell");

            foreach (DataGridViewColumn col in dgvinvoice.Columns) 
            {
                col.DefaultCellStyle.ForeColor = Color.Navy;
            }
            dgvinvoice.Columns[1].DefaultCellStyle.ForeColor = Color.Red;
            dgvinvoice.Columns[3].DefaultCellStyle.ForeColor = Color.DarkGreen;

            items.DataSource = new BindingSource(itemsData, null);
            items.DisplayMember = "value";
            items.ValueMember = "key";
            txtprice.Text = items.SelectedValue.ToString();



            textName.Focus();
            textName.Select();
            textName.SelectAll();
        }

        private void textDate_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void textDate_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                textDate.ContextMenu = new ContextMenu();
            }
        }

        private void txtTotal_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = true;
        }

        private void textName_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyData == Keys.Enter) 
            {
                items.Focus();
            }
        }

        private void items_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                txtQty.Focus();
                txtQty.SelectAll();
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (items.SelectedIndex <= -1) return;
            string item = items.Text;
            int qty = Convert.ToInt32(txtQty.Text);
            int prise = Convert.ToInt32(txtprice.Text);
            int suptotal = qty * prise;
            object[] row = { item, qty, prise, suptotal };
            dgvinvoice.Rows.Add(row);
            txtTotal.Text = (Convert.ToInt32(txtTotal.Text) + suptotal + "");
        }

        private void txtQty_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == Keys.Enter)
            {
                btnAdd.PerformClick();
                items.Focus();
                
            }
        }

        private void items_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtprice.Text = items.SelectedValue.ToString();
        }

        private void txtQty_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsDigit(e.KeyChar)&& !char.IsControl(e.KeyChar)) 
            {
                e.Handled = true; 
            }

        }

        string oldqty = "1";

        private void dgvinvoice_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if(dgvinvoice.CurrentRow != null)
            {
                oldqty = dgvinvoice.CurrentRow.Cells["colQty"].Value + "";
            }
        }

        private void dgvinvoice_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvinvoice.CurrentRow !=null)
            {
                string newqty = dgvinvoice.CurrentRow.Cells["colQty"].Value + "";

                if (oldqty == newqty) return;

                    if (!Regex.IsMatch(newqty, "^\\d+$")) 
                {
                    dgvinvoice.CurrentRow.Cells["colQty"].Value = oldqty;
                }
                else
                {
                    int p = (int)dgvinvoice.CurrentRow.Cells["colprice"].Value;
                    int q = Convert.ToInt32(newqty);
                    dgvinvoice.CurrentRow.Cells["colSubTotal"].Value = (q * p);

                    int newTotal = 0;
                    foreach(DataGridViewRow r in dgvinvoice.Rows)
                    {
                        newTotal += Convert.ToInt32(r.Cells["colSubTotal"].Value);
                    }
                    txtTotal.Text = newTotal + "";
                }

            }
        }

        private void btnPrint_Click(object sender, EventArgs e)
        {
            ((Form)printPreviewDialog1).WindowState = FormWindowState.Maximized;
            if(printPreviewDialog1.ShowDialog()== DialogResult.OK)
            {
                printDocument1.Print();
            }
        }

        private void printDocument1_PrintPage(object sender, System.Drawing.Printing.PrintPageEventArgs e)
        {
            float margin = 40;
            Font f = new Font("Arial", 18, FontStyle.Bold);
            string strNO = "#ON" + txtlvNum.Text;
            string strDate = "التاريخ" + textDate.Text;
            string strName = "مطلوب من السيد :" + textName.Text; 

            SizeF fontSizeNo = e.Graphics.MeasureString(strNO,f);
            SizeF fontSizeDate = e.Graphics.MeasureString(strDate, f);
            SizeF fontSizeName = e.Graphics.MeasureString(strName, f);

            e.Graphics.DrawImage(Properties.Resources.logo, 5, 5, 200, 200);

            e.Graphics.DrawString(strNO, f, Brushes.Red, (e.PageBounds.Width - fontSizeNo.Width)/2, margin);
            e.Graphics.DrawString(strDate, f, Brushes.Black, e.PageBounds.Width - fontSizeDate.Width - margin,margin+fontSizeNo.Height);
            e.Graphics.DrawString(strName, f, Brushes.Navy, e.PageBounds.Width - fontSizeName.Width - margin, margin + fontSizeNo.Height + fontSizeDate.Height);

            float preHeigh = margin + fontSizeNo.Height + fontSizeDate.Height + fontSizeName.Height + 80;
            e.Graphics.DrawRectangle(Pens.Black, margin, preHeigh, e.PageBounds.Width - margin * 2,e.PageBounds.Height-margin - preHeigh);

            float colHeigh = 50;

            float col1width = 300;
            float col2width = 125+ col1width;
            float col3width = 125 + col2width;
            float col4width = 125 + col3width;

            e.Graphics.DrawLine(Pens.Black, margin, preHeigh + colHeigh,e.PageBounds.Width - margin,preHeigh + colHeigh);

            e.Graphics.DrawString("الصنف", f, Brushes.Black, e.PageBounds.Width - margin * 2 - col1width, preHeigh);
            e.Graphics.DrawLine(Pens.Black, e.PageBounds.Width - margin * 2 - col1width, preHeigh, e.PageBounds.Width - margin * 2 - col1width, e.PageBounds.Height - margin);


            e.Graphics.DrawString("الكميه", f, Brushes.Black, e.PageBounds.Width - margin * 2 - col2width, preHeigh);
            e.Graphics.DrawLine(Pens.Black, e.PageBounds.Width - margin * 2 - col2width, preHeigh, e.PageBounds.Width - margin * 2 - col2width, e.PageBounds.Height - margin);

            e.Graphics.DrawString("السعر", f, Brushes.Black, e.PageBounds.Width - margin * 2 - col3width, preHeigh);
            e.Graphics.DrawLine(Pens.Black, e.PageBounds.Width - margin * 2 - col3width, preHeigh, e.PageBounds.Width - margin * 2 - col3width, e.PageBounds.Height - margin);

            e.Graphics.DrawString("اجمالي", f, Brushes.Black, e.PageBounds.Width - margin * 2 - col4width, preHeigh);


            ////////////////////////////////////// invoice content ///////////////////

            float rowsHeight = 50;

            for(int x = 0; x < dgvinvoice.Rows.Count;x += 1)
            {
                e.Graphics.DrawString(dgvinvoice.Rows[x].Cells[0].Value.ToString(), f, Brushes.Navy, e.PageBounds.Width - margin * 2 - col1width, preHeigh + rowsHeight);
                e.Graphics.DrawString(dgvinvoice.Rows[x].Cells[1].Value.ToString(), f, Brushes.Black, e.PageBounds.Width - margin * 2 - col2width, preHeigh + rowsHeight);
                e.Graphics.DrawString(dgvinvoice.Rows[x].Cells[2].Value.ToString(), f, Brushes.Black, e.PageBounds.Width - margin * 2 - col3width, preHeigh + rowsHeight);
                e.Graphics.DrawString(dgvinvoice.Rows[x].Cells[3].Value.ToString(), f, Brushes.Black, e.PageBounds.Width - margin * 2 - col4width, preHeigh + rowsHeight);


                e.Graphics.DrawLine(Pens.Black, margin, preHeigh + rowsHeight + colHeigh,e.PageBounds.Width-margin,preHeigh + rowsHeight + colHeigh);

                rowsHeight += 50;
            }
            e.Graphics.DrawString("الاجمالي", f, Brushes.Red, e.PageBounds.Width - margin * 2 - col3width, preHeigh + rowsHeight);
            e.Graphics.DrawString(txtTotal.Text, f, Brushes.Blue, e.PageBounds.Width - margin * 2 - col4width, preHeigh + rowsHeight);

            e.Graphics.DrawLine(Pens.Black, margin, preHeigh + rowsHeight + colHeigh, e.PageBounds.Width - margin, preHeigh + rowsHeight + colHeigh);

        }
    }
}
