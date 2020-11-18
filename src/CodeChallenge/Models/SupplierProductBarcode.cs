﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeChallenge.Models
{
    public class SupplierProductBarcode
    {
        public Supplier Supplier { get; set; }
        public Product Product { get; set; }
        public string Barcode { get; set; }
    }
}
