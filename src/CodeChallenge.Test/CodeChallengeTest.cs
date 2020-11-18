using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CodeChallenge.Test
{
    [TestClass]
    public class CodeChallengeTest
    {
        private InMemoryCatalogManager catalogManager;

        [TestInitialize]
        public void Setup()
        {
            catalogManager = new InMemoryCatalogManager(@"..\..\input\catalogA.csv",
              @"..\..\input\suppliersA.csv",
              @"..\..\input\barcodesA.csv");
        }

        [TestMethod]
        public void InitalLoad()
        {
            Assert.IsTrue(catalogManager.Catalogs.Count > 0);
            Assert.IsTrue(catalogManager.Suppliers.Count > 0);
            Assert.IsTrue(catalogManager.Barcodes.Count > 0);
        }

        [TestMethod]
        public void MergeAndExport()
        {
            catalogManager.Merge(@"..\..\input\catalogB.csv",
                @"..\..\input\suppliersB.csv",
                @"..\..\input\barcodesB.csv");

            var exportSuccess = catalogManager.ExportCatalog(@"..\..\output\result_output.csv");

            Assert.IsTrue(exportSuccess);
        }

        [TestMethod]
        public void AddNewProduct_New()
        {
            bool success = catalogManager.AddProduct("ABC-DEF-GHK",
                "This is a test", new[] { "BARCODE-1", "BARCODE-2" }, 1);
            Assert.IsTrue(success);
        }

        [TestMethod]
        public void AddNewProduct_Existing()
        {
            catalogManager.AddProduct("ABC-DEF-GHK",
                "This is a test", new[] { "BARCODE-1", "BARCODE-2" }, 1);
            bool success = catalogManager.AddProduct("ABC-DEF-GHK",
                "This is a test", new[] { "BARCODE-1", "BARCODE-2" }, 1);
            Assert.IsFalse(success);
        }

        [TestMethod]
        public void RemoveProduct_Existing()
        {
            bool removeSuccess = false;
            bool success = catalogManager.AddProduct("ABC-DEF-GHK",
                "This is a test", new[] { "BARCODE-1", "BARCODE-2" }, 1);

            if (success)
            {
                removeSuccess = catalogManager.RemoveProduct("ABC-DEF-GHK");
            }
            Assert.IsTrue(removeSuccess);
        }

        [TestMethod]
        public void RemoveProduct_NonExisting()
        {
            var removeSuccess = catalogManager.RemoveProduct("DUMMY-XYZ");

            Assert.IsFalse(removeSuccess);
        }

        [TestMethod]
        public void AddNewBarcodes_CorrectSKU_ShouldOK()
        {
            catalogManager.AddProduct("ABC-DEF-GHK",
               "This is a test", new[] { "BARCODE-1", "BARCODE-2" }, 1);

            bool success = catalogManager.AddBarcodes("ABC-DEF-GHK", new[] { "BARCODE-3", "BARCODE-4" }, 1);

            Assert.IsTrue(success);
        }

        [TestMethod]
        public void AddNewBarcodes_WrongSKU_ShouldFail()
        {
            catalogManager.AddProduct("ABC-DEF-GHK",
               "This is a test", new[] { "BARCODE-1", "BARCODE-2" }, 1);

            bool success = catalogManager.AddBarcodes("ABC-DEF-GHK-Z", new[] { "BARCODE-3", "BARCODE-4" }, 1);

            Assert.IsFalse(success);
        }

        [TestMethod]
        public void AddNewBarcodes_WrongSupplier_ShouldFail()
        {
            catalogManager.AddProduct("ABC-DEF-GHK",
               "This is a test", new[] { "BARCODE-1", "BARCODE-2" }, 1);

            bool success = catalogManager.AddBarcodes("ABC-DEF-GHK", new[] { "BARCODE-3", "BARCODE-4" }, 99999999);

            Assert.IsFalse(success);
        }
    }
}
