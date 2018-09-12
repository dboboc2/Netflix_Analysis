//
//  Netflix  Database  Application  using  N-Tier  Design.
//
// << Daniel Boboc >>
// U. of Illinois, Chicago
// CS341, Spring 2018
// Project 08
//


using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;




namespace proj8_dboboc2{
    static class Program{
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(){
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }



    }




}
