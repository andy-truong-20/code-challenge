using CodeChallenge.Models;
using System.Collections.Generic;

namespace CodeChallenge
{
    public interface ICatalogManager
    {
        bool AddBarcodes(string sku, string[] barcodes, int supplierID);
        bool AddProduct(string sku, string description, string[] barcodes, int supplierID);
        bool ExportCatalog(string outputPath);
        void Merge(string catalogBPath, string supplierBPath, string barcodeBPath);
        bool RemoveProduct(string sku);
    }
}