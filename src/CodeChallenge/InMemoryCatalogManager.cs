using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeChallenge.Models;

namespace CodeChallenge
{
    public class InMemoryCatalogManager: ICatalogManager
    {
        const string CATALOG_ORIGINAL = "A";

        public List<Product> Catalogs { get; } = new List<Product>();
        public List<Supplier> Suppliers { get; } = new List<Supplier>();
        public List<SupplierProductBarcode> Barcodes { get; } = new List<SupplierProductBarcode>();

        public InMemoryCatalogManager(string catalogPath,
            string supplierPath,
            string barcodePath)
        {
            Catalogs = LoadProducts(catalogPath, CATALOG_ORIGINAL);
            Suppliers = LoadSupplier(supplierPath);
            Barcodes = LoadBarcodes(barcodePath, Catalogs, Suppliers);
        }

        public void Merge(string catalogBPath, string supplierBPath, string barcodeBPath)
        {
            var catalogB = LoadProducts(catalogBPath, "B");
            var suppliersB = LoadSupplier(supplierBPath);
            var barcodesB = LoadBarcodes(barcodeBPath, catalogB, suppliersB);

            foreach (var productBarcode in barcodesB)
            {
                if (!Barcodes.Any(bc => bc.Barcode == productBarcode.Barcode))
                {
                    if (!Catalogs.Any(p => p.SKU == productBarcode.Product.SKU))
                    {
                        Catalogs.Add(productBarcode.Product);
                    }
                    Barcodes.Add(productBarcode);
                }
            }
        }

        public bool AddProduct(string sku, string description, string[] barcodes, int supplierID)
        {
            if (Catalogs.Any(p => p.SKU.Equals(sku, StringComparison.InvariantCultureIgnoreCase)))
            {
                return false;
            }
            else
            {
                Catalogs.Add(new Product() { SKU = sku, Description = description, Original = CATALOG_ORIGINAL });
                return AddBarcodes(sku, barcodes, supplierID);
            }
        }

        public bool RemoveProduct(string sku)
        {
            var product = Catalogs.FirstOrDefault(p => p.SKU.Equals(sku, StringComparison.InvariantCultureIgnoreCase));

            if (product != null)
            {
                var removeSuccess = Catalogs.Remove(product);
                Barcodes.RemoveAll(bc => bc.Product.SKU.Equals(sku, StringComparison.InvariantCultureIgnoreCase));

                return removeSuccess;
            }
            else
            {
                return false;
            }
        }

        public bool AddBarcodes(string sku, string[] barcodes, int supplierID)
        {
            var product = Catalogs.FirstOrDefault(p => p.SKU.Equals(sku, StringComparison.InvariantCultureIgnoreCase));
            var supplier = Suppliers.FirstOrDefault(s => s.ID == supplierID);

            if (product == null || supplier == null)
            {
                return false;
            }

            foreach (string barcode in barcodes)
            {
                if (!Barcodes.Any(bc => bc.Barcode.Equals(barcode, StringComparison.InvariantCultureIgnoreCase)))
                {
                    Barcodes.Add(new SupplierProductBarcode() { Barcode = barcode, Product = product, Supplier = supplier });
                }
            }

            return true;
        }

        public bool ExportCatalog(string outputPath)
        {
            try
            {
                File.WriteAllLines(outputPath, Catalogs.Select(p => $"{p.SKU},{p.Description},{p.Original}").Prepend("SKU,Description,Source"));
                return true;
            }
            catch
            {
                return false;
            }
        }

        private List<Product> LoadProducts(string productFilePath, string original) =>
            File.Exists(productFilePath)
                ? File.ReadLines(productFilePath).Skip(1).Select(line => new Product()
                {
                    SKU = line.Split(',')[0].Trim(),
                    Description = line.Split(',')[1].Trim(),
                    Original = original
                }).ToList()
                : new List<Product>();

        private List<Supplier> LoadSupplier(string supplierFilePath) =>
            File.Exists(supplierFilePath)
                ? File.ReadLines(supplierFilePath).Skip(1).Select(line => new Supplier()
                {
                    ID = int.Parse(line.Split(',')[0]),
                    Name = line.Split(',')[1].Trim()
                }).ToList()
                : new List<Supplier>();

        private List<SupplierProductBarcode> LoadBarcodes(string barcodeFilePath,
            List<Product> catalog,
            List<Supplier> suppliers) =>
           File.Exists(barcodeFilePath)
               ? File.ReadLines(barcodeFilePath).Skip(1).Select(line => new SupplierProductBarcode()
               {
                   Product = catalog.Find(p => p.SKU == (line.Split(',')[1].Trim())),
                   Supplier = suppliers.Find(s => s.ID == int.Parse(line.Split(',')[0])),
                   Barcode = line.Split(',')[2].Trim()
               }).ToList()
               : new List<SupplierProductBarcode>();
    }
}
