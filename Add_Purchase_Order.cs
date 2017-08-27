using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Business_Management_App
{
    public partial class Add_Purchase_Order : Form
    {
        private const int Product_Name_Column = 0;
        private const int Product_Quantity_Column = 1;
        private const int Product_BuyPrice_Column = 2;
        private const int Product_SellPrice_Column = 3;
        private const int Product_Category_Column = 4;
        private const int Product_totalPrice_Column = 5;
        private static double total_Purchase_list_Price = 0.0;
        ShopDataBaseDataContext database = new ShopDataBaseDataContext();
        public Add_Purchase_Order()
        {
            InitializeComponent();
            this.dataGridView1.Columns[Product_Quantity_Column].ValueType = typeof(int);
            this.dataGridView1.Columns[Product_BuyPrice_Column].ValueType = typeof(double);
            this.dataGridView1.Columns[Product_SellPrice_Column].ValueType = typeof(double);
            
        }

        private void Add_Purchase_Order_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'shopDataBaseDataSet.Product' table. You can move, or remove it, as needed.
            this.productTableAdapter.Fill(this.shopDataBaseDataSet.Product);
           // dataGridView1.Columns[Product_SellPrice_Column].Visible = false;
        }

        private void productBindingNavigatorSaveItem_Click(object sender, EventArgs e)
        {
            this.Validate();
            this.productBindingSource.EndEdit();
            this.tableAdapterManager.UpdateAll(this.shopDataBaseDataSet);

        }

        private void dataGridView1_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            TextBox autoText = e.Control as TextBox;
            //string currentColumnName = dataGridView1.CurrentCell.OwningColumn.Name;

            if (dataGridView1.CurrentCell.OwningColumn.Name.Equals("Column_Product_Name"))
            {   

                if (autoText != null)
                {
                    autoText.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    autoText.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    AutoCompleteStringCollection DataCollection = new AutoCompleteStringCollection();

                    var ProductNameArray = (from C in database.Products
                                            select C.Name);

                    foreach (string item in ProductNameArray)
                    {
                        DataCollection.Add(item);
                    }
                    autoText.AutoCompleteCustomSource = DataCollection;

                }

            }
            else if (dataGridView1.CurrentCell.OwningColumn.Name.Equals("Column_Category"))
            {
                if (autoText != null)
                {
                    autoText.AutoCompleteMode = AutoCompleteMode.SuggestAppend;
                    autoText.AutoCompleteSource = AutoCompleteSource.CustomSource;
                    AutoCompleteStringCollection DataCollection = new AutoCompleteStringCollection();

                    var ProductCategotyArray = (from C in database.Categories
                                                select C.Name);

                    foreach (string item in ProductCategotyArray)
                    {
                        DataCollection.Add(item);
                    }
                    autoText.AutoCompleteCustomSource = DataCollection;

                }
            }
            else
            {
                autoText.AutoCompleteCustomSource = null;
            }
            
           
           
        }
        

        private void dataGridView1_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            Console.WriteLine("abbas is testing");
            int rowIndex = dataGridView1.CurrentRow.Index;
            if (this.dataGridView1.CurrentCell.ColumnIndex == Product_Name_Column)
            {
                if (this.dataGridView1.CurrentCell.Value != null)
                {
                    if (ProductNameExistInDatabase(this.dataGridView1.CurrentCell.Value.ToString()))
                    {
                        var ProductDescription = (from C in database.Products
                                                  where C.Name.Equals(this.dataGridView1.CurrentCell.Value.ToString())
                                                  select C).First();
                        Console.WriteLine("hi");
                        dataGridView1.Rows[rowIndex].Cells[Product_Category_Column].Value = ProductDescription.Category;
                    }
                }
            }
            if (this.dataGridView1.CurrentCell.ColumnIndex == Product_BuyPrice_Column) {
                if (dataGridView1.Rows[rowIndex].Cells[Product_Quantity_Column].Value != null)
                {
                    if (!dataGridView1.Rows[rowIndex].Cells[Product_Quantity_Column].Value.ToString().Equals(""))
                    {
                        var totalUnitPrice = dataGridView1.Rows[rowIndex].Cells[Product_totalPrice_Column];
                        double buy_Price = Double.Parse(dataGridView1.Rows[rowIndex].Cells[Product_BuyPrice_Column].Value.ToString());
                        int Quanity = int.Parse(dataGridView1.Rows[rowIndex].Cells[Product_Quantity_Column].Value.ToString());
                        totalUnitPrice.Value = Quanity * buy_Price;
                        total_Purchase_list_Price += Double.Parse(totalUnitPrice.Value.ToString());
                        textBox6.Text = total_Purchase_list_Price.ToString();
                        Console.WriteLine(total_Purchase_list_Price.ToString());
                    }
                }
                
            }
            else if (this.dataGridView1.CurrentCell.ColumnIndex == Product_Quantity_Column)
            {
                if (dataGridView1.Rows[rowIndex].Cells[Product_BuyPrice_Column].Value != null)
                {
                    if (!dataGridView1.Rows[rowIndex].Cells[Product_BuyPrice_Column].Value.ToString().Equals(""))
                    {
                        var totalUnitPrice = dataGridView1.Rows[rowIndex].Cells[Product_totalPrice_Column];
                        double buy_Price = Double.Parse(dataGridView1.Rows[rowIndex].Cells[Product_BuyPrice_Column].Value.ToString());
                        int Quanity = int.Parse(dataGridView1.Rows[rowIndex].Cells[Product_Quantity_Column].Value.ToString());
                        totalUnitPrice.Value = Quanity * buy_Price;
                        total_Purchase_list_Price += Double.Parse(totalUnitPrice.Value.ToString());
                        textBox6.Text = total_Purchase_list_Price.ToString();
                        Console.WriteLine(total_Purchase_list_Price.ToString());
                    }
                }
               
            }
            //New Code after giving the code to ja3far
            
        }
        private void btn_Add_Product_Click(object sender, EventArgs e)
        {
            if (checkDataGridViewForNullValue())
                return;
            foreach (DataGridViewRow row in dataGridView1.Rows) {
                foreach (DataGridViewCell cell in row.Cells) {
                    if (cell.ColumnIndex == Product_Name_Column)
                    {
                        if (dataGridView1.Rows[cell.RowIndex].Cells[Product_BuyPrice_Column].Value != null)
                        {
                            int BuyPrice = int.Parse(dataGridView1.Rows[cell.RowIndex].Cells[Product_BuyPrice_Column].Value.ToString());
                            Console.WriteLine(BuyPrice);
                            if (ProductPriceExistInDatabase(BuyPrice, cell.Value.ToString()))
                            {
                                Console.WriteLine(cell.Value.ToString() + " with Price " + BuyPrice + " Exist in database so update quantity");
                                Product c = (from x in database.Products
                                              where x.Id == ProductId(BuyPrice, cell.Value.ToString())
                                              select x).First();
                                c.Quantity += int.Parse(dataGridView1.Rows[cell.RowIndex].Cells[Product_Quantity_Column].Value.ToString());
                                
                                database.SubmitChanges();

                            }
                            else
                            {
                                Console.WriteLine(cell.Value.ToString() + " Exist in database with a diffrent Price so insert as new Product");
                                Product result;
                                if (dataGridView1.Rows[cell.RowIndex].Cells[Product_SellPrice_Column].Value != null)
                                {
                                    result = new Product
                                    {
                                        Name = cell.Value.ToString(),
                                        Quantity = int.Parse(dataGridView1.Rows[cell.RowIndex].Cells[Product_Quantity_Column].Value.ToString()),
                                        Buy_Price = float.Parse(dataGridView1.Rows[cell.RowIndex].Cells[Product_BuyPrice_Column].Value.ToString()),
                                        Sell_Price = float.Parse(dataGridView1.Rows[cell.RowIndex].Cells[Product_SellPrice_Column].Value.ToString()),
                                        Category = dataGridView1.Rows[cell.RowIndex].Cells[Product_Category_Column].Value.ToString()

                                    };
                                }
                                else {
                                    result = new Product
                                    {
                                        Name = cell.Value.ToString(),
                                        Quantity = int.Parse(dataGridView1.Rows[cell.RowIndex].Cells[Product_Quantity_Column].Value.ToString()),
                                        Buy_Price = float.Parse(dataGridView1.Rows[cell.RowIndex].Cells[Product_BuyPrice_Column].Value.ToString()),                 
                                        Category = dataGridView1.Rows[cell.RowIndex].Cells[Product_Category_Column].Value.ToString()
                                        
                                    };
                                }
                                database.Products.InsertOnSubmit(result);
                                database.SubmitChanges();
                            }
                           
                        }
                    }
                }

            }

        }
        private bool checkDataGridViewForNullValue() {
            int RowCount = dataGridView1.Rows.Count;
            foreach (DataGridViewRow row in dataGridView1.Rows)
            {
                RowCount--;
                foreach (DataGridViewCell cell in row.Cells)
                {
                    if (cell.Value == null && cell.ColumnIndex != Product_SellPrice_Column)
                    {
                        MessageBox.Show("cell ("+(cell.RowIndex+1)+", "+(cell.ColumnIndex+1)+") is empty");
                        return true;
                    }
                }
                //TODO:check if it works for all cases
                if (RowCount == 1) return false;
            }
            return false;
        }

        //this method search in the Product table in the database
        //and return if the product exist
        private bool ProductNameExistInDatabase(string productName)
        {
            var Products = (from C in database.Products
                            where C.Name.Equals(productName)
                            select C.Name).ToArray<string>();

            return Products.Length != 0;
        }
        private int ProductId(int productBuyPrice, string ProductName)
        {
            var Products = (from C in database.Products
                            where C.Name.Equals(ProductName) && C.Buy_Price == productBuyPrice
                            select C).First();

            return Products.Id;
        }
        private bool ProductPriceExistInDatabase(int productBuyPrice,string ProductName)
        {
            var Products = (from C in database.Products
                            where C.Name.Equals(ProductName) && C.Buy_Price == productBuyPrice
                            select C.Name).ToArray();

            return Products.Length != 0;
        }
        
        
        
        private void dataGridView1_RowLeave(object sender, DataGridViewCellEventArgs e)
        {
            //dataGridView1.Columns[0].Visible = false;
        }

        private void dataGridView1_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("The value you entered is incorrect please rewrite it correctly");
            
        }

        private void btn_Delete_Product_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow row in dataGridView1.SelectedRows) {
                dataGridView1.Rows.RemoveAt(row.Index);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (buttonShowHide.Text == "Show Sell Price")
            {
                dataGridView1.Columns[Product_SellPrice_Column].Visible = true;
                buttonShowHide.Text = "Hide Sell Price";

            }
            else {
                dataGridView1.Columns[Product_SellPrice_Column].Visible = false;
                buttonShowHide.Text = "Show Sell Price";
            }

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
         
    }
}
