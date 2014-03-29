namespace CUP
{
	using System;
	
	/// 
	/// <summary> This class handles emitting generated code for the resulting parser.
	/// The various parse tables must be constructed, etc. before calling any 
	/// routines in this class.<p>  
	/// *
	/// Three classes are produced by this code:
	/// <dl>
	/// <dt> symbol constant class
	/// <dd>   this contains constant declarations for each terminal (and 
	/// optionally each non-terminal).
	/// <dt> action class
	/// <dd>   this non-public class contains code to invoke all the user actions 
	/// that were embedded in the parser specification.
	/// <dt> parser class
	/// <dd>   the specialized parser class consisting primarily of some user 
	/// supplied general and initialization code, and the parse tables.
	/// </dl><p>
	/// *
	/// Three parse tables are created as part of the parser class:
	/// <dl>
	/// <dt> production table
	/// <dd>   lists the LHS non terminal number, and the length of the RHS of 
	/// each production.
	/// <dt> action table
	/// <dd>   for each state of the parse machine, gives the action to be taken
	/// (shift, reduce, or error) under each lookahead symbol.<br>
	/// <dt> reduce-goto table
	/// <dd>   when a reduce on a given production is taken, the parse stack is 
	/// popped back a number of elements corresponding to the RHS of the 
	/// production.  This reveals a prior state, which we transition out 
	/// of under the LHS non terminal symbol for the production (as if we
	/// had seen the LHS symbol rather than all the symbols matching the 
	/// RHS).  This table is indexed by non terminal numbers and indicates 
	/// how to make these transitions. 
	/// </dl><p>
	/// 
	/// In addition to the method interface, this class maintains a series of 
	/// public global variables and flags indicating how misc. parts of the code 
	/// and other output is to be produced, and counting things such as number of 
	/// conflicts detected (see the source code and public variables below for
	/// more details).<p> 
	/// *
	/// This class is "static" (contains only static data and methods).<p> 
	/// *
	/// </summary>
	/// <seealso cref=" CUP.main
	/// "/>
	/// <version> last update: 11/25/95
	/// </version>
	/// <author> Scott Hudson
	/// 
	/// </author>
	
	/* Major externally callable routines here include:
	symbols               - emit the symbol constant class 
	parser                - emit the parser class
	
	In addition the following major internal routines are provided:
	emit_package          - emit a package declaration
	emit_action_code      - emit the class containing the user's actions 
	emit_production_table - emit declaration and init for the production table
	do_action_table       - emit declaration and init for the action table
	do_reduce_table       - emit declaration and init for the reduce-goto table
	
	Finally, this class uses a number of public instance variables to communicate
	optional parameters and flags used to control how code is generated,
	as well as to report counts of various things (such as number of conflicts
	detected).  These include:
	
	prefix                  - a prefix string used to prefix names that would 
	otherwise "pollute" someone else's name space.
	package_name            - name of the package emitted code is placed in 
	(or null for an unnamed package.
	symbol_const_class_name - name of the class containing symbol constants.
	parser_class_name       - name of the class for the resulting parser.
	action_code             - user supplied declarations and other code to be 
	placed in action class.
	parser_code             - user supplied declarations and other code to be 
	placed in parser class.
	init_code               - user supplied code to be executed as the parser 
	is being initialized.
	scan_code               - user supplied code to get the next Symbol.
	start_production        - the start production for the grammar.
	import_list             - list of imports for use with action class.
	num_conflicts           - number of conflicts detected. 
	nowarn                  - true if we are not to issue warning messages.
	not_reduced             - count of number of productions that never reduce.
	unused_term             - count of unused terminal symbols.
	unused_non_term         - count of unused non terminal symbols.
	*_time                  - a series of symbols indicating how long various
	sub-parts of code generation took (used to produce
	optional time reports in main).*/
	
	public class emit
	{
		
		/*-----------------------------------------------------------*/
		/*--- Constructor(s) ----------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Only constructor is private so no instances can be created. 
		/// </summary>
		public emit()
		{
		}
		
		/*-----------------------------------------------------------*/
		/*--- Static (Class) Variables ------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>The prefix placed on names that pollute someone else's name space. 
		/// </summary>
		public static System.String prefix = "CUP_";
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Package that the resulting code goes into (null is used for unnamed). 
		/// </summary>
		public static System.String namespace_name = null;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Name of the generated class for symbol constants. 
		/// </summary>
		public static System.String symbol_const_class_name = "sym";
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Name of the generated parser class. 
		/// </summary>
		public static System.String parser_class_name = "parser";
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>User declarations for direct inclusion in user action class. 
		/// </summary>
		public static System.String action_code = null;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>User declarations for direct inclusion in parser class. 
		/// </summary>
		public static System.String parser_code = null;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>User code for user_init() which is called during parser initialization. 
		/// </summary>
		public static System.String init_code = null;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>User code for scan() which is called to get the next Symbol. 
		/// </summary>
		public static System.String scan_code = null;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>The start production of the grammar. 
		/// </summary>
		public static production start_production = null;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>List of imports (Strings containing class names) to go with actions. 
		/// </summary>
		public static CUP.runtime.SymbolStack import_list = new CUP.runtime.SymbolStack();
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Number of conflict found while building tables. 
		/// </summary>
		public static int num_conflicts = 0;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Do we skip warnings? 
		/// </summary>
		public static bool nowarn = false;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Count of the number on non-reduced productions found. 
		/// </summary>
		public static int not_reduced = 0;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Count of unused terminals. 
		/// </summary>
		public static int unused_term = 0;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Count of unused non terminals. 
		/// </summary>
		public static int unused_non_term = 0;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/* Timing values used to produce timing report in main.*/
		
		/// <summary>Time to produce symbol constant class. 
		/// </summary>
		public static long symbols_time = 0;
		
		/// <summary>Time to produce parser class. 
		/// </summary>
		public static long parser_time = 0;
		
		/// <summary>Time to produce action code class. 
		/// </summary>
		public static long action_code_time = 0;
		
		/// <summary>Time to produce the production table. 
		/// </summary>
		public static long production_table_time = 0;
		
		/// <summary>Time to produce the action table. 
		/// </summary>
		public static long action_table_time = 0;
		
		/// <summary>Time to produce the reduce-goto table. 
		/// </summary>
		public static long goto_table_time = 0;
		
		/* frankf 6/18/96 */
		protected internal static bool _lr_values;
		
		/// <summary>whether or not to emit code for left and right values 
		/// </summary>
		public static bool lr_values()
		{
			return _lr_values;
		}
		protected internal static void  set_lr_values(bool b)
		{
			_lr_values = b;
		}
		
		/*-----------------------------------------------------------*/
		/*--- General Methods ---------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>Build a string with the standard prefix. 
		/// </summary>
		/// <param name="str">string to prefix.
		/// 
		/// </param>
		protected internal static System.String pre(System.String str)
		{
			return prefix + parser_class_name + "_" + str;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Emit a package spec if the user wants one. 
		/// </summary>
		/// <param name="out">stream to produce output on.
		/// 
		/// </param>
		protected internal static void  emit_package(System.IO.StreamWriter out_Renamed)
		{
			/* generate a package spec if we have a name for one */
			if (namespace_name != null)
			{
				out_Renamed.WriteLine("namespace " + namespace_name + " {"); out_Renamed.WriteLine();
			}
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Emit code for the symbol constant class, optionally including non terms,
		/// if they have been requested.  
		/// </summary>
		/// <param name="out">           stream to produce output on.
		/// </param>
		/// <param name="emit_non_terms">do we emit constants for non terminals?
		/// </param>
		/// <param name="sym_interface"> should we emit an interface, rather than a class?
		/// 
		/// </param>
		public static void  symbols(System.IO.StreamWriter out_Renamed, bool emit_non_terms, bool sym_interface)
		{
			terminal term;
			non_terminal nt;
			System.String class_or_interface = (sym_interface)?"interface":"class";
			
			long start_time = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
			
			/* top of file */
			out_Renamed.WriteLine();
			out_Renamed.WriteLine("//----------------------------------------------------");
			out_Renamed.WriteLine("// The following code was generated by " + version.title_str);
			out_Renamed.WriteLine("// " + System.DateTime.Now);
			out_Renamed.WriteLine("//----------------------------------------------------");
			out_Renamed.WriteLine();
			out_Renamed.WriteLine("using System;");
			emit_package(out_Renamed);
			
			/* class header */
			out_Renamed.WriteLine("/** CUP generated " + class_or_interface + " containing symbol constants. */");
			out_Renamed.WriteLine("public " + class_or_interface + " " + symbol_const_class_name + " {");
			
			out_Renamed.WriteLine("  /* terminals */");
			
			/* walk over the terminals */ /* later might sort these */
			 for (System.Collections.IEnumerator e = terminal.all(); e.MoveNext(); )
			{
				term = (terminal) e.Current;
				
				/* output a constant decl for the terminal */
				out_Renamed.WriteLine("  public static readonly int " + term.name_Renamed_Method() + " = " + term.index() + ";");
			}
			
			/* do the non terminals if they want them (parser doesn't need them) */
			if (emit_non_terms)
			{
				out_Renamed.WriteLine();
				out_Renamed.WriteLine("  /* non terminals */");
				
				/* walk over the non terminals */ /* later might sort these */
				 for (System.Collections.IEnumerator e = non_terminal.all(); e.MoveNext(); )
				{
					nt = (non_terminal) e.Current;
					
					/* output a constant decl for the terminal */
					out_Renamed.WriteLine("  static final int " + nt.name_Renamed_Method() + " = " + nt.index() + ";");
				}
			}
			
			/* end of class */
			out_Renamed.WriteLine("}");
			// end of namespace
			out_Renamed.WriteLine("}");
			out_Renamed.WriteLine();
			
			symbols_time = (System.DateTime.Now.Ticks - 621355968000000000) / 10000 - start_time;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Emit code for the non-public class holding the actual action code. 
		/// </summary>
		/// <param name="out">       stream to produce output on.
		/// </param>
		/// <param name="start_prod">the start production of the grammar.
		/// 
		/// </param>
		protected internal static void  emit_action_code(System.IO.StreamWriter out_Renamed, production start_prod)
		{
			production prod;
			
			long start_time = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
			
			/* class header */
			out_Renamed.WriteLine();
			out_Renamed.WriteLine("/** Cup generated class to encapsulate user supplied action code.*/");
			out_Renamed.WriteLine("public class " + pre("actions") + " {");
			
			/* user supplied code */
			if (action_code != null)
			{
				out_Renamed.WriteLine();
				out_Renamed.WriteLine(action_code);
			}
			
			/* field for parser object */
			out_Renamed.WriteLine("  private " + parser_class_name + " parser;");
			
			/* constructor */
			out_Renamed.WriteLine();
			out_Renamed.WriteLine("  /** Constructor */");
			out_Renamed.WriteLine("  public " + pre("actions") + "(" + parser_class_name + " parser) {");
			out_Renamed.WriteLine("    this.parser = parser;");
			out_Renamed.WriteLine("  }");
			
			/* action method head */
			out_Renamed.WriteLine();
			out_Renamed.WriteLine("  /** Method with the actual generated action code. */");
			out_Renamed.WriteLine("  public CUP.runtime.Symbol " + pre("do_action") + "(");
			out_Renamed.WriteLine("    int                        " + pre("act_num,"));
			out_Renamed.WriteLine("    CUP.runtime.lr_parser " + pre("parser,"));
			out_Renamed.WriteLine("    CUP.runtime.SymbolStack            " + pre("stack,"));
			out_Renamed.WriteLine("    int                        " + pre("top)"));
			out_Renamed.WriteLine("    {");
			
			/* declaration of result symbol */
			/* New declaration!! now return Symbol
			6/13/96 frankf */
			out_Renamed.WriteLine("      /* Symbol object for return from actions */");
			out_Renamed.WriteLine("      CUP.runtime.Symbol " + pre("result") + ";");
			out_Renamed.WriteLine();
			
			/* switch top */
			out_Renamed.WriteLine("      /* select the action based on the action number */");
			out_Renamed.WriteLine("      switch (" + pre("act_num") + ")");
			out_Renamed.WriteLine("        {");
			
			/* emit action code for each production as a separate case */
			 for (System.Collections.IEnumerator p = production.all(); p.MoveNext(); )
			{
				prod = (production) p.Current;
				
				/* case label */
				out_Renamed.WriteLine("          /*. . . . . . . . . . . . . . . . . . . .*/");
				out_Renamed.WriteLine("          case " + prod.index() + ": // " + prod.to_simple_string());
				
				/* give them their own block to work in */
				out_Renamed.WriteLine("            {");
				
				/* create the result symbol */
				/*make the variable RESULT which will point to the new Symbol (see below)
				and be changed by action code
				6/13/96 frankf */
				 string strSymType = prod.lhs().the_symbol().stack_type().Trim();
				 string strPrimitives="int;float;double;short;char;byte;decimal;sbyte;bool;ushort;uint;long;ulong";
				 if(strPrimitives.IndexOf(strSymType) > -1) 
				 {
					 out_Renamed.WriteLine("              " + strSymType + " RESULT ;");
				 }
				 else 
				 {
					 out_Renamed.WriteLine("              " + strSymType + " RESULT = null;");
				 }
				
				/* Add code to propagate RESULT assignments that occur in
				* action code embedded in a production (ie, non-rightmost
				* action code). 24-Mar-1998 CSA
				*/
				 for (int i = 0; i < prod.rhs_length(); i++)
				{
					// only interested in non-terminal symbols.
					if (!(prod.rhs(i) is symbol_part))
						continue;
					symbol s = ((symbol_part) prod.rhs(i)).the_symbol();
					if (!(s is non_terminal))
						continue;
					// skip this non-terminal unless it corresponds to
					// an embedded action production.
					if (((non_terminal) s).is_embedded_action == false)
						continue;
					// OK, it fits.  Make a conditional assignment to RESULT.
					int index = prod.rhs_length() - i - 1; // last rhs is on top.
					out_Renamed.WriteLine("              " + "// propagate RESULT from " + s.name_Renamed_Method());
					out_Renamed.WriteLine("              " + "if ( " + "((CUP.runtime.Symbol) " + emit.pre("stack") + ".Peek(" + emit.pre("top") + "-" + index + ")).Value != null )");
					out_Renamed.WriteLine("                " + "RESULT = " + "(" + prod.lhs().the_symbol().stack_type() + ") " + "((CUP.runtime.Symbol) " + emit.pre("stack") + ".Peek(" + emit.pre("top") + "-" + index + ")).Value;");
				}
				
				/* if there is an action string, emit it */
				if (prod.action() != null && prod.action().code_string() != null && !prod.action().Equals(""))
					out_Renamed.WriteLine(prod.action().code_string());
				
				/* here we have the left and right values being propagated.  
				must make this a command line option.
				frankf 6/18/96 */
				
				/* Create the code that assigns the left and right values of
				the new Symbol that the production is reducing to */
				if (emit.lr_values())
				{
					int loffset;
					System.String leftstring, rightstring;
					int roffset = 0;
					rightstring = "((CUP.runtime.Symbol)" + emit.pre("stack") + ".Peek(" + emit.pre("top") + "-" + roffset + ")).right";
					if (prod.rhs_length() == 0)
						leftstring = rightstring;
					else
					{
						loffset = prod.rhs_length() - 1;
						leftstring = "((CUP.runtime.Symbol)" + emit.pre("stack") + ".Peek(" + emit.pre("top") + "-" + loffset + ")).left";
					}
					out_Renamed.WriteLine("              " + pre("result") + " = new CUP.runtime.Symbol(" + prod.lhs().the_symbol().index() + "/*" + prod.lhs().the_symbol().name_Renamed_Method() + "*/" + ", " + leftstring + ", " + rightstring + ", RESULT);");
				}
				else
				{
					out_Renamed.WriteLine("              " + pre("result") + " = new CUP.runtime.Symbol(" + prod.lhs().the_symbol().index() + "/*" + prod.lhs().the_symbol().name_Renamed_Method() + "*/" + ", RESULT);");
				}
				
				/* end of their block */
				out_Renamed.WriteLine("            }");
				
				/* if this was the start production, do action for accept */
				if (prod == start_prod)
				{
					out_Renamed.WriteLine("          /* ACCEPT */");
					out_Renamed.WriteLine("          " + pre("parser") + ".done_parsing();");
				}
				
				/* code to return lhs symbol */
				out_Renamed.WriteLine("          return " + pre("result") + ";");
				out_Renamed.WriteLine();
			}
			
			/* end of switch */
			out_Renamed.WriteLine("          /* . . . . . .*/");
			out_Renamed.WriteLine("          default:");
			out_Renamed.WriteLine("            throw new Exception(");
			out_Renamed.WriteLine("               \"Invalid action number found in " + "internal parse table\");");
			out_Renamed.WriteLine();
			out_Renamed.WriteLine("        }");
			
			/* end of method */
			out_Renamed.WriteLine("    }");
			
			/* end of class */
			out_Renamed.WriteLine("}");
			out_Renamed.WriteLine();
			
			action_code_time = (System.DateTime.Now.Ticks - 621355968000000000) / 10000 - start_time;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Emit the production table. 
		/// </summary>
		/// <param name="out">stream to produce output on.
		/// 
		/// </param>
		protected internal static void  emit_production_table(System.IO.StreamWriter out_Renamed)
		{
			production[] all_prods;
			production prod;
			
			long start_time = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
			
			/* collect up the productions in order */
			all_prods = new production[production.number()];
			 for (System.Collections.IEnumerator p = production.all(); p.MoveNext(); )
			{
				prod = (production) p.Current;
				all_prods[prod.index()] = prod;
			}
			
			// make short[,]
			short[][] prod_table = new short[production.number()][];
			 for (int i = 0; i < production.number(); i++)
			{
				prod_table[i] = new short[2];
			}
			 for (int i = 0; i < production.number(); i++)
			{
				prod = all_prods[i];
				// { lhs symbol , rhs size }
				prod_table[i][0] = (short) prod.lhs().the_symbol().index();
				prod_table[i][1] = (short) prod.rhs_length();
			}
			/* do the top of the table */
			out_Renamed.WriteLine();
			out_Renamed.WriteLine("  /** Production table. */");
			out_Renamed.Write("  private static String[] _strProductionTable = ");
			do_table_as_string(out_Renamed, prod_table);
			out_Renamed.WriteLine(";");
			out_Renamed.WriteLine("  protected static readonly short[][] _production_table  = CUP.runtime.lr_parser.unpackFromStrings(_strProductionTable);");
			// out_Renamed.Write("    unpackFromStrings(");
			// do_table_as_string(out_Renamed, prod_table);
			// out_Renamed.WriteLine(");");
			
			/* do the public accessor method */
			out_Renamed.WriteLine();
			out_Renamed.WriteLine("  /** Access to production table. */");
			out_Renamed.WriteLine("  public override short[][] production_table() " + "{return _production_table;}");
			
			production_table_time = (System.DateTime.Now.Ticks - 621355968000000000) / 10000 - start_time;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Emit the action table. 
		/// </summary>
		/// <param name="out">            stream to produce output on.
		/// </param>
		/// <param name="act_tab">        the internal representation of the action table.
		/// </param>
		/// <param name="compact_reduces">do we use the most frequent reduce as default?
		/// 
		/// </param>
		protected internal static void  do_action_table(System.IO.StreamWriter out_Renamed, parse_action_table act_tab, bool compact_reduces)
		{
			parse_action_row row;
			parse_action act;
			int red;
			
			long start_time = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
			
			/* collect values for the action table */
			short[][] action_table = new short[act_tab.num_states()][];
			/* do each state (row) of the action table */
			 for (int i = 0; i < act_tab.num_states(); i++)
			{
				/* get the row */
				row = act_tab.under_state[i];
				
				/* determine the default for the row */
				if (compact_reduces)
					row.compute_default();
				else
					row.default_reduce = - 1;
				
				/* make temporary table for the row. */
				short[] temp_table = new short[2 * CUP.parse_action_row.size()];
				int nentries = 0;
				
				/* do each column */
				 for (int j = 0; j < CUP.parse_action_row.size(); j++)
				{
					/* extract the action from the table */
					act = row.under_term[j];
					
					/* skip error entries these are all defaulted out */
					if (act.kind() != parse_action.ERROR)
					{
						/* first put in the symbol index, then the actual entry */
						
						/* shifts get positive entries of state number + 1 */
						if (act.kind() == parse_action.SHIFT)
						{
							/* make entry */
							temp_table[nentries++] = (short) j;
							temp_table[nentries++] = (short) (((shift_action) act).shift_to().index() + 1);
						}
						else if (act.kind() == parse_action.REDUCE)
						{
							/* if its the default entry let it get defaulted out */
							red = ((reduce_action) act).reduce_with().index();
							if (red != row.default_reduce)
							{
								/* make entry */
								temp_table[nentries++] = (short) j;
								temp_table[nentries++] = (short) (- (red + 1));
							}
						}
						else if (act.kind() == parse_action.NONASSOC)
						{
							/* do nothing, since we just want a syntax error */
						}
						else
							throw new internal_error("Unrecognized action code " + act.kind() + " found in parse table");
					}
				}
				
				/* now we know how big to make the row */
				action_table[i] = new short[nentries + 2];
				Array.Copy(temp_table, 0, action_table[i], 0, nentries);
				
				/* finish off the row with a default entry */
				action_table[i][nentries++] = - 1;
				if (row.default_reduce != - 1)
					action_table[i][nentries++] = (short) (- (row.default_reduce + 1));
				else
					action_table[i][nentries++] = 0;
			}
			
			/* finish off the init of the table */
			out_Renamed.WriteLine();
			out_Renamed.WriteLine("  /** Parse-action table. */");
			out_Renamed.Write("  private static String[] _strActionTable = ");
			do_table_as_string(out_Renamed, action_table);
			out_Renamed.WriteLine(";");
			out_Renamed.WriteLine("  protected static short[][] _action_table = CUP.runtime.lr_parser.unpackFromStrings(_strActionTable);");
			// out_Renamed.Write("    unpackFromStrings(");
			// do_table_as_string(out_Renamed, action_table);
			// out_Renamed.WriteLine(");");
			
			/* do the public accessor method */
			out_Renamed.WriteLine();
			out_Renamed.WriteLine("  /** Access to parse-action table. */");
			out_Renamed.WriteLine("  public override short[][] action_table() {return _action_table;}");
			
			action_table_time = (System.DateTime.Now.Ticks - 621355968000000000) / 10000 - start_time;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Emit the reduce-goto table. 
		/// </summary>
		/// <param name="out">    stream to produce output on.
		/// </param>
		/// <param name="red_tab">the internal representation of the reduce-goto table.
		/// 
		/// </param>
		protected internal static void  do_reduce_table(System.IO.StreamWriter out_Renamed, parse_reduce_table red_tab)
		{
			lalr_state goto_st;
			// parse_action act;
			
			long start_time = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
			
			/* collect values for reduce-goto table */
			short[][] reduce_goto_table = new short[red_tab.num_states()][];
			/* do each row of the reduce-goto table */
			 for (int i = 0; i < red_tab.num_states(); i++)
			{
				/* make temporary table for the row. */
				short[] temp_table = new short[2 * CUP.parse_reduce_row.size()];
				int nentries = 0;
				/* do each entry in the row */
				 for (int j = 0; j < CUP.parse_reduce_row.size(); j++)
				{
					/* get the entry */
					goto_st = red_tab.under_state[i].under_non_term[j];
					
					/* if we have none, skip it */
					if (goto_st != null)
					{
						/* make entries for the index and the value */
						temp_table[nentries++] = (short) j;
						temp_table[nentries++] = (short) goto_st.index();
					}
				}
				/* now we know how big to make the row. */
				reduce_goto_table[i] = new short[nentries + 2];
				Array.Copy(temp_table, 0, reduce_goto_table[i], 0, nentries);
				
				/* end row with default value */
				reduce_goto_table[i][nentries++] = - 1;
				reduce_goto_table[i][nentries++] = - 1;
			}
			
			/* emit the table. */
			out_Renamed.WriteLine();
			out_Renamed.WriteLine("  /** <code>reduce_goto</code> table. */");
			out_Renamed.Write("  private static String[] _strGotoTable = ");
			do_table_as_string(out_Renamed, reduce_goto_table);
			out_Renamed.WriteLine(";");
			out_Renamed.WriteLine("  protected static readonly short[][] _reduce_table = CUP.runtime.lr_parser.unpackFromStrings(_strGotoTable);");
			//out_Renamed.Write("    unpackFromStrings(");
			//do_table_as_string(out_Renamed, reduce_goto_table);
			//out_Renamed.WriteLine(");");
			
			/* do the public accessor method */
			out_Renamed.WriteLine();
			out_Renamed.WriteLine("  /** Access to <code>reduce_goto</code> table. */");
			out_Renamed.WriteLine("  public override short[][] reduce_table() {return _reduce_table;}");
			out_Renamed.WriteLine();
			
			goto_table_time = (System.DateTime.Now.Ticks - 621355968000000000) / 10000 - start_time;
		}
		
		// print a string array encoding the given short[][] array.
		protected internal static void  do_table_as_string(System.IO.StreamWriter out_Renamed, short[][] sa)
		{
			out_Renamed.WriteLine("new String[] {");
			out_Renamed.Write("    \"");
			int nchar = 0, nbytes = 0;
			nbytes += do_escaped(out_Renamed, (char) (sa.Length >> 16));
			nchar = do_newline(out_Renamed, nchar, nbytes);
			nbytes += do_escaped(out_Renamed, (char) (sa.Length & 0xFFFF));
			nchar = do_newline(out_Renamed, nchar, nbytes);
			 for (int i = 0; i < sa.Length; i++)
			{
				nbytes += do_escaped(out_Renamed, (char) (sa[i].Length >> 16));
				nchar = do_newline(out_Renamed, nchar, nbytes);
				nbytes += do_escaped(out_Renamed, (char) (sa[i].Length & 0xFFFF));
				nchar = do_newline(out_Renamed, nchar, nbytes);
				 for (int j = 0; j < sa[i].Length; j++)
				{
					// contents of string are (value+2) to allow for common -1, 0 cases
					// (UTF-8 encoding is most efficient for 0<c<0x80)
					nbytes += do_escaped(out_Renamed, (char) (2 + sa[i][j]));
					nchar = do_newline(out_Renamed, nchar, nbytes);
				}
			}
			out_Renamed.Write("\" }");
		}
		// split string if it is very long; start new line occasionally for neatness
		protected internal static int do_newline(System.IO.StreamWriter out_Renamed, int nchar, int nbytes)
		{
			if (nbytes > 65500)
			{
				out_Renamed.WriteLine("\", "); out_Renamed.Write("    \"");
			}
			else if (nchar > 11)
			{
				out_Renamed.WriteLine("\" +"); out_Renamed.Write("    \"");
			}
			else
				return nchar + 1;
			return 0;
		}
		// output an escape sequence for the given character code.
		protected internal static int do_escaped(System.IO.StreamWriter out_Renamed, char c)
		{
			System.Text.StringBuilder escape = new System.Text.StringBuilder();
			if (c <= 0xFF)
			{
				escape.Append(System.Convert.ToString(c, 16));
				while (escape.Length < 4)
				{
					escape.Insert(0, '0');
				}
				escape.Insert(0, 'x');
			}
			else
			{
				escape.Append(System.Convert.ToString(c, 16));
				while (escape.Length < 4)
				{
					escape.Insert(0, '0');
				}
				escape.Insert(0, 'u');
			}
			escape.Insert(0, '\\');
			out_Renamed.Write(escape.ToString());
			
			// return number of bytes this takes up in UTF-8 encoding.
			if (c == 0)
				return 2;
			if (c >= 0x01 && c <= 0x7F)
				return 1;
			if (c >= 0x80 && c <= 0x7FF)
				return 2;
			return 3;
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Emit the parser subclass with embedded tables. 
		/// </summary>
		/// <param name="out">            stream to produce output on.
		/// </param>
		/// <param name="action_table">   internal representation of the action table.
		/// </param>
		/// <param name="reduce_table">   internal representation of the reduce-goto table.
		/// </param>
		/// <param name="start_st">       start state of the parse machine.
		/// </param>
		/// <param name="start_prod">     start production of the grammar.
		/// </param>
		/// <param name="compact_reduces">do we use most frequent reduce as default?
		/// </param>
		/// <param name="suppress_scanner">should scanner be suppressed for compatibility?
		/// 
		/// </param>
		public static void  parser(System.IO.StreamWriter out_Renamed, parse_action_table action_table, parse_reduce_table reduce_table, int start_st, production start_prod, bool compact_reduces, bool suppress_scanner)
		{
			long start_time = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
			
			/* top of file */
			out_Renamed.WriteLine();
			out_Renamed.WriteLine("//----------------------------------------------------");
			out_Renamed.WriteLine("// The following code was generated by " + version.title_str);
			out_Renamed.WriteLine("// " + System.DateTime.Now);
			out_Renamed.WriteLine("//----------------------------------------------------");
			out_Renamed.WriteLine();
			out_Renamed.WriteLine("using System;");
			emit_package(out_Renamed);
			
			/* user supplied imports */
			for (int i = 0; i < import_list.Count; i++) 
			{
				out_Renamed.WriteLine("using " + import_list.Peek(import_list.Count - (i + 1)) + ";");
			}
			
			/* class header */
			out_Renamed.WriteLine();
			out_Renamed.WriteLine("///<summary>" + version.title_str + " generated parser." + "</summary>");
			out_Renamed.WriteLine("///<version>" + System.DateTime.Now + "</version>");
			out_Renamed.WriteLine("///");
			out_Renamed.WriteLine("public class " + parser_class_name + " : CUP.runtime.lr_parser {");
			
			/* constructors [CSA/davidm, 24-jul-99] */
			out_Renamed.WriteLine();
			out_Renamed.WriteLine("  /** Default constructor. */");
			out_Renamed.WriteLine("  public " + parser_class_name + "() : base() {}");
			if (!suppress_scanner)
			{
				out_Renamed.WriteLine();
				out_Renamed.WriteLine("  /** Constructor which sets the default scanner. */");
				out_Renamed.WriteLine("  public " + parser_class_name + "(CUP.runtime.Scanner s) : base(s) {}");
			}
			
			/* emit the various tables */
			emit_production_table(out_Renamed);
			do_action_table(out_Renamed, action_table, compact_reduces);
			do_reduce_table(out_Renamed, reduce_table);
			
			/* instance of the action encapsulation class */
			out_Renamed.WriteLine("  /** Instance of action encapsulation class. */");
			out_Renamed.WriteLine("  protected " + pre("actions") + " action_obj;");
			out_Renamed.WriteLine();
			
			/* action object initializer */
			out_Renamed.WriteLine("  /** Action encapsulation object initializer. */");
			out_Renamed.WriteLine("  protected override void init_actions()");
			out_Renamed.WriteLine("    {");
			out_Renamed.WriteLine("      action_obj = new " + pre("actions") + "(this);");
			out_Renamed.WriteLine("    }");
			out_Renamed.WriteLine();
			
			/* access to action code */
			out_Renamed.WriteLine("  /** Invoke a user supplied parse action. */");
			out_Renamed.WriteLine("  public override CUP.runtime.Symbol do_action(");
			out_Renamed.WriteLine("    int                        act_num,");
			out_Renamed.WriteLine("    CUP.runtime.lr_parser parser,");
			out_Renamed.WriteLine("    CUP.runtime.SymbolStack   stack,");
			out_Renamed.WriteLine("    int                        top)");
			out_Renamed.WriteLine("  {");
			out_Renamed.WriteLine("    /* call code in generated class */");
			out_Renamed.WriteLine("    return action_obj." + pre("do_action(") + "act_num, parser, stack, top);");
			out_Renamed.WriteLine("  }");
			out_Renamed.WriteLine("");
			
			
			/* method to tell the parser about the start state */
			out_Renamed.WriteLine("  /** Indicates start state. */");
			out_Renamed.WriteLine("  public override int start_state() {return " + start_st + ";}");
			
			/* method to indicate start production */
			out_Renamed.WriteLine("  /** Indicates start production. */");
			out_Renamed.WriteLine("  public override int start_production() {return " + start_production.index() + ";}");
			out_Renamed.WriteLine();
			
			/* methods to indicate EOF and error symbol indexes */
			out_Renamed.WriteLine("  /** <code>EOF</code> Symbol index. */");
			out_Renamed.WriteLine("  public override int EOF_sym() {return " + terminal.EOF.index() + ";}");
			out_Renamed.WriteLine();
			out_Renamed.WriteLine("  /** <code>error</code> Symbol index. */");
			out_Renamed.WriteLine("  public override int error_sym() {return " + terminal.error.index() + ";}");
			out_Renamed.WriteLine();
			
			/* user supplied code for user_init() */
			if (init_code != null)
			{
				out_Renamed.WriteLine();
				out_Renamed.WriteLine("  /** User initialization code. */");
				out_Renamed.WriteLine("  public override void user_init()");
				out_Renamed.WriteLine("    {");
				out_Renamed.WriteLine(init_code);
				out_Renamed.WriteLine("    }");
			}
			
			/* user supplied code for scan */
			if (scan_code != null)
			{
				out_Renamed.WriteLine();
				out_Renamed.WriteLine("  /** Scan to get the next Symbol. */");
				out_Renamed.WriteLine("  public override CUP.runtime.Symbol scan()");
				out_Renamed.WriteLine("    {");
				out_Renamed.WriteLine(scan_code);
				out_Renamed.WriteLine("    }");
			}
			
			/* user supplied code */
			if (parser_code != null)
			{
				out_Renamed.WriteLine();
				out_Renamed.WriteLine(parser_code);
			}
			
			/* end of class */
			out_Renamed.WriteLine("}");
			
			/* put out the action code class */
			emit_action_code(out_Renamed, start_prod);
			
			// End of namespace
			out_Renamed.WriteLine("}");

			parser_time = (System.DateTime.Now.Ticks - 621355968000000000) / 10000 - start_time;
		}
		
		/*-----------------------------------------------------------*/
	}
}