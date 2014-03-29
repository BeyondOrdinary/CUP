using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;
using System.Data;
using CUP.runtime;

namespace simple_calc
{
	/// <summary>
	/// Summary description for Form1.
	/// </summary>
	public class SimpleCalc : System.Windows.Forms.Form
	{
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox _textFormula;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox _textResult;
		private System.Windows.Forms.Button _btnCalc;
		private System.Windows.Forms.CheckBox _checkDebug;
        private System.Windows.Forms.Label label3;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public SimpleCalc()
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Resources.ResourceManager resources = new System.Resources.ResourceManager(typeof(SimpleCalc));
            this.label1 = new System.Windows.Forms.Label();
            this._textFormula = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this._textResult = new System.Windows.Forms.TextBox();
            this._btnCalc = new System.Windows.Forms.Button();
            this._checkDebug = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(16, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(48, 16);
            this.label1.TabIndex = 0;
            this.label1.Text = "Formula:";
            // 
            // _textFormula
            // 
            this._textFormula.Location = new System.Drawing.Point(64, 16);
            this._textFormula.Name = "_textFormula";
            this._textFormula.Size = new System.Drawing.Size(208, 20);
            this._textFormula.TabIndex = 1;
            this._textFormula.Text = "1+2+3-4*5";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(8, 72);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(40, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "Result:";
            // 
            // _textResult
            // 
            this._textResult.Location = new System.Drawing.Point(64, 72);
            this._textResult.Name = "_textResult";
            this._textResult.Size = new System.Drawing.Size(208, 20);
            this._textResult.TabIndex = 3;
            this._textResult.Text = "";
            // 
            // _btnCalc
            // 
            this._btnCalc.Location = new System.Drawing.Point(200, 40);
            this._btnCalc.Name = "_btnCalc";
            this._btnCalc.TabIndex = 4;
            this._btnCalc.Text = "Calculate";
            this._btnCalc.Click += new System.EventHandler(this._btnCalc_Click);
            // 
            // _checkDebug
            // 
            this._checkDebug.Location = new System.Drawing.Point(96, 40);
            this._checkDebug.Name = "_checkDebug";
            this._checkDebug.TabIndex = 5;
            this._checkDebug.Text = "Debug";
            this._checkDebug.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label3
            // 
            this.label3.Location = new System.Drawing.Point(0, 120);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(288, 23);
            this.label3.TabIndex = 6;
            this.label3.Text = "(c) 2003 Beyond Ordinary Software Solutions";
            // 
            // SimpleCalc
            // 
            this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
            this.ClientSize = new System.Drawing.Size(292, 145);
            this.Controls.Add(this.label3);
            this.Controls.Add(this._checkDebug);
            this.Controls.Add(this._btnCalc);
            this.Controls.Add(this._textResult);
            this.Controls.Add(this._textFormula);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SimpleCalc";
            this.Text = "CUP Simple Calc Demonstration";
            this.ResumeLayout(false);

        }
		#endregion

		/// <summary>
		/// The main entry point for the application.
		/// </summary>
		[STAThread]
		static void Main() 
		{
			Application.Run(new SimpleCalc());
		}

		private void _btnCalc_Click(object sender, System.EventArgs e)
		{
			/* create a parsing object */
			string strText = _textFormula.Text + ";";
			System.Text.Encoding encoding = System.Text.Encoding.Default;
			byte[] bytes = encoding.GetBytes(strText);
			System.IO.MemoryStream stream = new System.IO.MemoryStream(bytes);
			parser parser_obj = new parser(new scanner(stream));
			
			/* open input files, etc. here */
			Symbol parse_tree = null;
			
			if(_checkDebug.Checked) 
			{
				try
				{
					parse_tree = parser_obj.debug_parse();
				}
				catch (System.Exception ex)
				{
					Console.WriteLine("Exception=" + ex);
					Console.WriteLine(ex.StackTrace.ToString());
					_textResult.Text = ex.Message;
					parse_tree = null;
				}			
			}
			else 
			{
				try
				{
					parse_tree = parser_obj.parse();
				}
				catch (System.Exception ex)
				{
					Console.WriteLine("Exception=" + ex);
					Console.WriteLine(ex.StackTrace.ToString());
					_textResult.Text = ex.Message;
					parse_tree = null;
				}			
			}
			try 
			{
				if(parse_tree != null) 
				{
					_textResult.Text = parse_tree.Value.ToString();
				}
			}
			catch(Exception ex) 
			{
				Console.WriteLine(ex.StackTrace.ToString());
				_textResult.Text = ex.Message;
			}
		}

		private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
		{
		
		}
	}
}
