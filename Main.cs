namespace CUP
{
	using System;
	
	/// <summary>This class serves as the main driver for the JavaCup system.
	/// It accepts user options and coordinates overall control flow.
	/// The main flow of control includes the following activities: 
	/// <ul>
	/// <li> Parse user supplied arguments and options.
	/// <li> Open output files.
	/// <li> Parse the specification from standard input.
	/// <li> Check for unused terminals, non-terminals, and productions.
	/// <li> Build the state machine, tables, etc.
	/// <li> Output the generated code.
	/// <li> Close output files.
	/// <li> Print a summary if requested.
	/// </ul>
	/// *
	/// Options to the main program include: <dl>
	/// <dt> -namespace name  
	/// <dd> specify namespace generated classes go in [default none]
	/// <dt> -parser name   
	/// <dd> specify parser class name [default "parser"]
	/// <dt> -symbols name  
	/// <dd> specify name for symbol constant class [default "sym"]
	/// <dt> -interface
	/// <dd> emit symbol constant <i>interface</i>, rather than class
	/// <dt> -nonterms      
	/// <dd> put non terminals in symbol constant class
	/// <dt> -expect #      
	/// <dd> number of conflicts expected/allowed [default 0]
	/// <dt> -compact_red   
	/// <dd> compact tables by defaulting to most frequent reduce
	/// <dt> -nowarn        
	/// <dd> don't warn about useless productions, etc.
	/// <dt> -nosummary     
	/// <dd> don't print the usual summary of parse states, etc.
	/// <dt> -progress      
	/// <dd> print messages to indicate progress of the system
	/// <dt> -time          
	/// <dd> print time usage summary
	/// <dt> -dump_grammar  
	/// <dd> produce a dump of the symbols and grammar
	/// <dt> -dump_states   
	/// <dd> produce a dump of parse state machine
	/// <dt> -dump_tables   
	/// <dd> produce a dump of the parse tables
	/// <dt> -dump          
	/// <dd> produce a dump of all of the above
	/// <dt> -debug         
	/// <dd> turn on debugging messages within JavaCup 
	/// <dt> -nopositions
	/// <dd> don't generate the positions code
	/// <dt> -noscanner
	/// <dd> don't refer to CUP.runtime.Scanner in the parser
	/// (for compatibility with old runtimes)
	/// <dt> -version
	/// <dd> print version information for JavaCUP and halt.
	/// </dl>
	/// *
	/// </summary>
	/// <version> last updated: 7/3/96
	/// </version>
	/// <author>  Frank Flannery
	/// 
	/// </author>
	
	public class ParserGenerator
	{
		
		/*-----------------------------------------------------------*/
		/*--- Constructor(s) ----------------------------------------*/
		/*-----------------------------------------------------------*/
		/// <summary>Only constructor is private, so we do not allocate any instances of this
		/// class. 
		/// </summary>
		public ParserGenerator()
		{
		}
		
		/*-------------------------*/
		/* Options set by the user */
		/*-------------------------*/
		/// <summary>User option -- do we print progress messages. 
		/// </summary>
		protected internal static bool print_progress = true;
		/// <summary>User option -- do we produce a dump of the state machine 
		/// </summary>
		protected internal static bool opt_dump_states = false;
		/// <summary>User option -- do we produce a dump of the parse tables 
		/// </summary>
		protected internal static bool opt_dump_tables = false;
		/// <summary>User option -- do we produce a dump of the grammar 
		/// </summary>
		protected internal static bool opt_dump_grammar = false;
		/// <summary>User option -- do we show timing information as a part of the summary 
		/// </summary>
		protected internal static bool opt_show_timing = false;
		/// <summary>User option -- do we run produce extra debugging messages 
		/// </summary>
		protected internal static bool opt_do_debug = false;
		/// <summary>User option -- do we compact tables by making most common reduce the 
		/// default action 
		/// </summary>
		protected internal static bool opt_compact_red = false;
		/// <summary>User option -- should we include non terminal symbol numbers in the 
		/// symbol constant class. 
		/// </summary>
		protected internal static bool include_non_terms = false;
		/// <summary>User option -- do not print a summary. 
		/// </summary>
		protected internal static bool no_summary = false;
		/// <summary>User option -- number of conflicts to expect 
		/// </summary>
		protected internal static int expect_conflicts = 0;
		
		/* frankf added this 6/18/96 */
		/// <summary>User option -- should generator generate code for left/right values? 
		/// </summary>
		protected internal static bool lr_values = true;
		
		/// <summary>User option -- should symbols be put in a class or an interface? [CSA]
		/// </summary>
		protected internal static bool sym_interface = false;
		
		/// <summary>User option -- should generator suppress references to
		/// CUP.runtime.Scanner for compatibility with old runtimes? 
		/// </summary>
		protected internal static bool suppress_scanner = false;
		
		/*----------------------------------------------------------------------*/
		/* Timing data (not all of these time intervals are mutually exclusive) */
		/*----------------------------------------------------------------------*/
		/// <summary>Timing data -- when did we start 
		/// </summary>
		protected internal static long start_time = 0;
		/// <summary>Timing data -- when did we end preliminaries 
		/// </summary>
		protected internal static long prelim_end = 0;
		/// <summary>Timing data -- when did we end parsing 
		/// </summary>
		protected internal static long parse_end = 0;
		/// <summary>Timing data -- when did we end checking 
		/// </summary>
		protected internal static long check_end = 0;
		/// <summary>Timing data -- when did we end dumping 
		/// </summary>
		protected internal static long dump_end = 0;
		/// <summary>Timing data -- when did we end state and table building 
		/// </summary>
		protected internal static long build_end = 0;
		/// <summary>Timing data -- when did we end nullability calculation 
		/// </summary>
		protected internal static long nullability_end = 0;
		/// <summary>Timing data -- when did we end first set calculation 
		/// </summary>
		protected internal static long first_end = 0;
		/// <summary>Timing data -- when did we end state machine construction 
		/// </summary>
		protected internal static long machine_end = 0;
		/// <summary>Timing data -- when did we end table construction 
		/// </summary>
		protected internal static long table_end = 0;
		/// <summary>Timing data -- when did we end checking for non-reduced productions 
		/// </summary>
		protected internal static long reduce_check_end = 0;
		/// <summary>Timing data -- when did we finish emitting code 
		/// </summary>
		protected internal static long emit_end = 0;
		/// <summary>Timing data -- when were we completely done 
		/// </summary>
		protected internal static long final_time = 0;
		
		/* Additional timing information is also collected in emit */
		
		/*-----------------------------------------------------------*/
		/*--- Main Program ------------------------------------------*/
		/*-----------------------------------------------------------*/
		
		/// <summary>The main driver for the system. 
		/// </summary>
		/// <param name="argv">an array of strings containing command line arguments.
		/// 
		/// </param>
		[STAThread]
		public static void  Main(string[] argv)
		{
			try 
			{
				bool did_output = false;
			
				start_time = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
			
				/* process user options and arguments */
				parse_args(argv);
			
				if(input_file == null) 
				{
					Console.WriteLine("ParserGenerator only accepts file input.");
					System.Environment.Exit(-1);
				}

				lexer.SetStream(input_file);

				/* frankf 6/18/96
				hackish, yes, but works */
				emit.set_lr_values(lr_values);
				/* open output files */
				if (print_progress)
					System.Console.Error.WriteLine("Opening files...");
		
				prelim_end = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
			
				/* parse spec into internal data structures */
				if (print_progress)
					System.Console.Error.WriteLine("Parsing specification from standard input...");
				parse_grammar_spec();
			
				parse_end = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
			
				/* don't proceed unless we are error free */
				if (lexer.error_count == 0)
				{
					/* check for unused bits */
					if (print_progress)
						System.Console.Error.WriteLine("Checking specification...");
					check_unused();
				
					check_end = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
				
					/* build the state machine and parse tables */
					if (print_progress)
						System.Console.Error.WriteLine("Building parse tables...");
					build_parser();
				
					build_end = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
				
					/* output the generated code, if # of conflicts permits */
					if (lexer.error_count != 0)
					{
						// conflicts! don't emit code, don't dump tables.
						opt_dump_tables = false;
					}
					else
					{
						// everything's okay, emit parser.
						if (print_progress)
							System.Console.Error.WriteLine("Writing parser...");
						open_files();
						emit_parser();
						did_output = true;
					}
				}
				/* fix up the times to make the summary easier */
				emit_end = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
			
				/* do requested dumps */
				if (opt_dump_grammar)
					dump_grammar();
				if (opt_dump_states)
					dump_machine();
				if (opt_dump_tables)
					dump_tables();
			
				dump_end = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
			
				/* close input/output files */
				if (print_progress)
					System.Console.Error.WriteLine("Closing files...");
				close_files();
			
				/* produce a summary if desired */
				if (!no_summary)
					emit_summary(did_output);
			
				/* If there were errors during the run,
				* exit with non-zero status (makefile-friendliness). --CSA */
				if (lexer.error_count != 0)
					System.Environment.Exit(100);
			}
			catch(Exception ex)
			{
				Console.WriteLine("Exception: {0}", ex);
				Console.WriteLine(ex.StackTrace);
			}
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Print a "usage message" that described possible command line options, 
		/// then exit.
		/// </summary>
		/// <param name="message">a specific error message to preface the usage message by.
		/// 
		/// </param>
		protected internal static void  usage(string message)
		{
			System.Console.Error.WriteLine();
			System.Console.Error.WriteLine(message);
			System.Console.Error.WriteLine();
			System.Console.Error.WriteLine("Usage: " + version.program_name + " [options] [filename]\n" + "  and expects a specification file on standard input if no filename is given.\n" + "  Legal options include:\n" + "    -namespace name  specify namespace generated classes go in [default none]\n" + "    -parser name   specify parser class name [default \"parser\"]\n" + "    -symbols name  specify name for symbol constant class [default \"sym\"]\n" + "    -interface     put symbols in an interface, rather than a class\n" + "    -nonterms      put non terminals in symbol constant class\n" + "    -expect #      number of conflicts expected/allowed [default 0]\n" + "    -compact_red   compact tables by defaulting to most frequent reduce\n" + "    -nowarn        don't warn about useless productions, etc.\n" + "    -nosummary     don't print the usual summary of parse states, etc.\n" + "    -nopositions   don't propagate the left and right token position values\n" + "    -noscanner     don't refer to CUP.runtime.Scanner\n" + "    -progress      print messages to indicate progress of the system\n" + "    -time          print time usage summary\n" + "    -dump_grammar  produce a human readable dump of the symbols and grammar\n" + "    -dump_states   produce a dump of parse state machine\n" + "    -dump_tables   produce a dump of the parse tables\n" + "    -dump          produce a dump of all of the above\n" + "    -version       print the version information for CUP and exit\n");
			System.Environment.Exit(1);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Parse command line options and arguments to set various user-option
		/// flags and variables. 
		/// </summary>
		/// <param name="argv">the command line arguments to be parsed.
		/// 
		/// </param>
		protected internal static void  parse_args(string[] argv)
		{
			int len = argv.Length;
			int i;
			
			/* parse the options */
			 for (i = 0; i < len; i++)
			{
				/* try to get the various options */
				if (argv[i].Equals("-namespace"))
				{
					/* must have an arg */
					if (++i >= len || argv[i].StartsWith("-") || argv[i].EndsWith(".cup"))
						usage("-namespace must have a name argument");
					
					/* record the name */
					emit.namespace_name = argv[i];
				}
				else if (argv[i].Equals("-parser"))
				{
					/* must have an arg */
					if (++i >= len || argv[i].StartsWith("-") || argv[i].EndsWith(".cup"))
						usage("-parser must have a name argument");
					
					/* record the name */
					emit.parser_class_name = argv[i];
				}
				else if (argv[i].Equals("-symbols"))
				{
					/* must have an arg */
					if (++i >= len || argv[i].StartsWith("-") || argv[i].EndsWith(".cup"))
						usage("-symbols must have a name argument");
					
					/* record the name */
					emit.symbol_const_class_name = argv[i];
				}
				else if (argv[i].Equals("-nonterms"))
				{
					include_non_terms = true;
				}
				else if (argv[i].Equals("-expect"))
				{
					/* must have an arg */
					if (++i >= len || argv[i].StartsWith("-") || argv[i].EndsWith(".cup"))
						usage("-expect must have a name argument");
					
					/* record the number */
					try
					{
						expect_conflicts = System.Int32.Parse(argv[i]);
					}
					catch (System.FormatException)
					{
						usage("-expect must be followed by a decimal integer");
					}
				}
				else if (argv[i].Equals("-compact_red"))
					opt_compact_red = true;
				else if (argv[i].Equals("-nosummary"))
					no_summary = true;
				else if (argv[i].Equals("-nowarn"))
					emit.nowarn = true;
				else if (argv[i].Equals("-dump_states"))
					opt_dump_states = true;
				else if (argv[i].Equals("-dump_tables"))
					opt_dump_tables = true;
				else if (argv[i].Equals("-progress"))
					print_progress = true;
				else if (argv[i].Equals("-dump_grammar"))
					opt_dump_grammar = true;
				else if (argv[i].Equals("-dump"))
					opt_dump_states = opt_dump_tables = opt_dump_grammar = true;
				else if (argv[i].Equals("-time"))
					opt_show_timing = true;
				else if (argv[i].Equals("-debug"))
					opt_do_debug = true;
				else if (argv[i].Equals("-nopositions"))
					lr_values = false;
				else if (argv[i].Equals("-interface"))
					sym_interface = true;
				else if (argv[i].Equals("-noscanner"))
					suppress_scanner = true;
				else if (argv[i].Equals("-version"))
				{
					System.Console.Out.WriteLine(version.title_str);
					System.Environment.Exit(1);
				}
				else if (!argv[i].StartsWith("-") && i == len - 1)
				{
					/* use input from file. */
					try
					{
						input_file = new System.IO.BufferedStream(new System.IO.FileStream(argv[i], System.IO.FileMode.Open, System.IO.FileAccess.Read));
					}
					catch (System.IO.FileNotFoundException)
					{
						usage("Unable to open \"" + argv[i] + "\" for input");
					}
				}
				else
				{
					usage("Unrecognized option \"" + argv[i] + "\"");
				}
			}
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/*-------*/
		/* Files */
		/*-------*/
		
		/// <summary>Input file.  This is a buffered version of System.in. 
		/// </summary>
		protected internal static System.IO.Stream       input_file;
		
		/// <summary>Output file for the parser class. 
		/// </summary>
		protected internal static System.IO.StreamWriter parser_class_file;
		
		/// <summary>Output file for the symbol constant class. 
		/// </summary>
		protected internal static System.IO.StreamWriter symbol_class_file;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Open various files used by the system. 
		/// </summary>
		protected internal static void  open_files()
		{
			System.IO.FileInfo fil;
			System.String out_name;
			
			/* open each of the output files */
			
			/* parser class */
			out_name = emit.parser_class_name + ".cs";
			fil = new System.IO.FileInfo(out_name);
			try
			{
				parser_class_file = new System.IO.StreamWriter(new System.IO.BufferedStream(new System.IO.FileStream(fil.FullName, System.IO.FileMode.Create), 4096));
			}
			catch (System.Exception)
			{
				System.Console.Error.WriteLine("Can't open \"" + out_name + "\" for output");
				System.Environment.Exit(3);
			}
			
			/* symbol constants class */
			out_name = emit.symbol_const_class_name + ".cs";
			fil = new System.IO.FileInfo(out_name);
			try
			{
				symbol_class_file = new System.IO.StreamWriter(new System.IO.BufferedStream(new System.IO.FileStream(fil.FullName, System.IO.FileMode.Create), 4096));
			}
			catch (System.Exception)
			{
				System.Console.Error.WriteLine("Can't open \"" + out_name + "\" for output");
				System.Environment.Exit(4);
			}
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Close various files used by the system. 
		/// </summary>
		protected internal static void  close_files()
		{
			if (input_file != null)
				input_file.Close();
			if (parser_class_file != null)
				parser_class_file.Close();
			if (symbol_class_file != null)
				symbol_class_file.Close();
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Parse the grammar specification from standard input.  This produces
		/// sets of terminal, non-terminals, and productions which can be accessed
		/// via static variables of the respective classes, as well as the setting
		/// of various variables (mostly in the emit class) for small user supplied
		/// items such as the code to scan with.
		/// </summary>
		protected internal static void  parse_grammar_spec()
		{
			parser parser_obj;
			
			/* create a parser and parse with it */
			parser_obj = new parser();
			try
			{
				if (opt_do_debug)
					parser_obj.debug_parse();
				else
					parser_obj.parse();
			}
			catch (System.Exception e)
			{
				/* something threw an exception.  catch it and emit a message so we 
				have a line number to work with, then re-throw it */
				lexer.emit_error("Internal error: Unexpected exception");
				Console.WriteLine("Internal Error: {0}", e.ToString());
				Console.WriteLine(e.StackTrace);
				throw e;
			}
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Check for unused symbols.  Unreduced productions get checked when
		/// tables are created.
		/// </summary>
		protected internal static void  check_unused()
		{
			terminal term;
			non_terminal nt;
			
			/* check for unused terminals */
			//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
			 for (System.Collections.IEnumerator t = terminal.all(); t.MoveNext(); )
			{
				//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
				term = (terminal) t.Current;
				
				/* don't issue a message for EOF */
				if (term == terminal.EOF)
					continue;
				
				/* or error */
				if (term == terminal.error)
					continue;
				
				/* is this one unused */
				if (term.use_count() == 0)
				{
					/* count it and warn if we are doing warnings */
					emit.unused_term++;
					if (!emit.nowarn)
					{
						System.Console.Error.WriteLine("Warning: Terminal \"" + term.name_Renamed_Method() + "\" was declared but never used");
						lexer.warning_count++;
					}
				}
			}
			
			/* check for unused non terminals */
			//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
			 for (System.Collections.IEnumerator n = non_terminal.all(); n.MoveNext(); )
			{
				//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
				nt = (non_terminal) n.Current;
				
				/* is this one unused */
				if (nt.use_count() == 0)
				{
					/* count and warn if we are doing warnings */
					emit.unused_term++;
					if (!emit.nowarn)
					{
						System.Console.Error.WriteLine("Warning: Non terminal \"" + nt.name_Renamed_Method() + "\" was declared but never used");
						lexer.warning_count++;
					}
				}
			}
			
		}
		
		/* . . . . . . . . . . . . . . . . . . . . . . . . .*/
		/* . . Internal Results of Generating the Parser . .*/
		/* . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Start state in the overall state machine. 
		/// </summary>
		protected internal static lalr_state start_state;
		
		/// <summary>Resulting parse action table. 
		/// </summary>
		protected internal static parse_action_table action_table;
		
		/// <summary>Resulting reduce-goto table. 
		/// </summary>
		protected internal static parse_reduce_table reduce_table;
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Build the (internal) parser from the previously parsed specification.
		/// This includes:<ul>
		/// <li> Computing nullability of non-terminals.
		/// <li> Computing first sets of non-terminals and productions.
		/// <li> Building the viable prefix recognizer machine.
		/// <li> Filling in the (internal) parse tables.
		/// <li> Checking for unreduced productions.
		/// </ul>
		/// </summary>
		protected internal static void  build_parser()
		{
			/* compute nullability of all non terminals */
			if (opt_do_debug || print_progress)
				System.Console.Error.WriteLine("  Computing non-terminal nullability...");
			non_terminal.compute_nullability();
			
			nullability_end = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
			
			/* compute first sets of all non terminals */
			if (opt_do_debug || print_progress)
				System.Console.Error.WriteLine("  Computing first sets...");
			non_terminal.compute_first_sets();
			
			first_end = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
			
			/* build the LR viable prefix recognition machine */
			if (opt_do_debug || print_progress)
				System.Console.Error.WriteLine("  Building state machine...");
			start_state = lalr_state.build_machine(emit.start_production);
			
			machine_end = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
			
			/* build the LR parser action and reduce-goto tables */
			if (opt_do_debug || print_progress)
				System.Console.Error.WriteLine("  Filling in tables...");
			action_table = new parse_action_table();
			reduce_table = new parse_reduce_table();
			 for (System.Collections.IEnumerator st = lalr_state.all(); st.MoveNext(); )
			{
				lalr_state lst = (lalr_state) st.Current;
				lst.build_table_entries(action_table, reduce_table);
			}
			
			table_end = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
			
			/* check and warn for non-reduced productions */
			if (opt_do_debug || print_progress)
				System.Console.Error.WriteLine("  Checking for non-reduced productions...");
			action_table.check_reductions();
			
			reduce_check_end = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
			
			/* if we have more conflicts than we expected issue a message and die */
			if (emit.num_conflicts > expect_conflicts)
			{
				System.Console.Error.WriteLine("*** More conflicts encountered than expected " + "-- parser generation aborted");
				lexer.error_count++; // indicate the problem.
				// we'll die on return, after clean up.
			}
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Call the emit routines necessary to write out the generated parser. 
		/// </summary>
		protected internal static void  emit_parser()
		{
			emit.symbols(symbol_class_file, include_non_terms, sym_interface);
			emit.parser(parser_class_file, action_table, reduce_table, start_state.index(), emit.start_production, opt_compact_red, suppress_scanner);
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Helper routine to optionally return a plural or non-plural ending. 
		/// </summary>
		/// <param name="val">the numerical value determining plurality.
		/// 
		/// </param>
		protected internal static string plural(int val)
		{
			if (val == 1)
				return "";
			else
				return "s";
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Emit a long summary message to standard error (System.err) which 
		/// summarizes what was found in the specification, how many states were
		/// produced, how many conflicts were found, etc.  A detailed timing 
		/// summary is also produced if it was requested by the user.
		/// </summary>
		/// <param name="output_produced">did the system get far enough to generate code.
		/// 
		/// </param>
		protected internal static void  emit_summary(bool output_produced)
		{
			final_time = (System.DateTime.Now.Ticks - 621355968000000000) / 10000;
			
			if (no_summary)
				return ;
			
			System.Console.Error.WriteLine("------- " + version.title_str + " Parser Generation Summary -------");
			
			/* error and warning count */
			System.Console.Error.WriteLine("  " + lexer.error_count + " error" + plural(lexer.error_count) + " and " + lexer.warning_count + " warning" + plural(lexer.warning_count));
			
			/* basic stats */
			System.Console.Error.Write("  " + terminal.number() + " terminal" + plural(terminal.number()) + ", ");
			System.Console.Error.Write(non_terminal.number() + " non-terminal" + plural(non_terminal.number()) + ", and ");
			System.Console.Error.WriteLine(production.number() + " production" + plural(production.number()) + " declared, ");
			System.Console.Error.WriteLine("  producing " + lalr_state.number() + " unique parse states.");
			
			/* unused symbols */
			System.Console.Error.WriteLine("  " + emit.unused_term + " terminal" + plural(emit.unused_term) + " declared but not used.");
			System.Console.Error.WriteLine("  " + emit.unused_non_term + " non-terminal" + plural(emit.unused_term) + " declared but not used.");
			
			/* productions that didn't reduce */
			System.Console.Error.WriteLine("  " + emit.not_reduced + " production" + plural(emit.not_reduced) + " never reduced.");
			
			/* conflicts */
			System.Console.Error.WriteLine("  " + emit.num_conflicts + " conflict" + plural(emit.num_conflicts) + " detected" + " (" + expect_conflicts + " expected).");
			
			/* code location */
			if (output_produced)
				System.Console.Error.WriteLine("  Code written to \"" + emit.parser_class_name + ".cs\", and \"" + emit.symbol_const_class_name + ".cs\".");
			else
				System.Console.Error.WriteLine("  No code produced.");
			
			if (opt_show_timing)
				show_times();
			
			System.Console.Error.WriteLine("---------------------------------------------------- (" + version.version_str + ")");
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Produce the optional timing summary as part of an overall summary. 
		/// </summary>
		protected internal static void  show_times()
		{
			long total_time = final_time - start_time;
			
			System.Console.Error.WriteLine(". . . . . . . . . . . . . . . . . . . . . . . . . ");
			System.Console.Error.WriteLine("  Timing Summary");
			System.Console.Error.WriteLine("    Total time       " + timestr(final_time - start_time, total_time));
			System.Console.Error.WriteLine("      Startup        " + timestr(prelim_end - start_time, total_time));
			System.Console.Error.WriteLine("      Parse          " + timestr(parse_end - prelim_end, total_time));
			if (check_end != 0)
				System.Console.Error.WriteLine("      Checking       " + timestr(check_end - parse_end, total_time));
			if (check_end != 0 && build_end != 0)
				System.Console.Error.WriteLine("      Parser Build   " + timestr(build_end - check_end, total_time));
			if (nullability_end != 0 && check_end != 0)
				System.Console.Error.WriteLine("        Nullability  " + timestr(nullability_end - check_end, total_time));
			if (first_end != 0 && nullability_end != 0)
				System.Console.Error.WriteLine("        First sets   " + timestr(first_end - nullability_end, total_time));
			if (machine_end != 0 && first_end != 0)
				System.Console.Error.WriteLine("        State build  " + timestr(machine_end - first_end, total_time));
			if (table_end != 0 && machine_end != 0)
				System.Console.Error.WriteLine("        Table build  " + timestr(table_end - machine_end, total_time));
			if (reduce_check_end != 0 && table_end != 0)
				System.Console.Error.WriteLine("        Checking     " + timestr(reduce_check_end - table_end, total_time));
			if (emit_end != 0 && build_end != 0)
				System.Console.Error.WriteLine("      Code Output    " + timestr(emit_end - build_end, total_time));
			if (emit.symbols_time != 0)
				System.Console.Error.WriteLine("        Symbols      " + timestr(emit.symbols_time, total_time));
			if (emit.parser_time != 0)
				System.Console.Error.WriteLine("        Parser class " + timestr(emit.parser_time, total_time));
			if (emit.action_code_time != 0)
				System.Console.Error.WriteLine("          Actions    " + timestr(emit.action_code_time, total_time));
			if (emit.production_table_time != 0)
				System.Console.Error.WriteLine("          Prod table " + timestr(emit.production_table_time, total_time));
			if (emit.action_table_time != 0)
				System.Console.Error.WriteLine("          Action tab " + timestr(emit.action_table_time, total_time));
			if (emit.goto_table_time != 0)
				System.Console.Error.WriteLine("          Reduce tab " + timestr(emit.goto_table_time, total_time));
			
			System.Console.Error.WriteLine("      Dump Output    " + timestr(dump_end - emit_end, total_time));
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Helper routine to format a decimal based display of seconds and
		/// percentage of total time given counts of milliseconds.   Note: this
		/// is broken for use with some instances of negative time (since we don't 
		/// use any negative time here, we let if be for now).
		/// </summary>
		/// <param name="time_val">  the value being formatted (in ms).
		/// </param>
		/// <param name="total_time">total time percentages are calculated against (in ms).
		/// 
		/// </param>
		protected internal static string timestr(long time_val, long total_time)
		{
			bool neg;
			long ms = 0;
			long sec = 0;
			long percent10;
			System.String pad;
			
			/* work with positives only */
			neg = time_val < 0;
			if (neg)
				time_val = - time_val;
			
			/* pull out seconds and ms */
			ms = time_val % 1000;
			sec = time_val / 1000;
			
			/* construct a pad to blank fill seconds out to 4 places */
			if (sec < 10)
				pad = "   ";
			else if (sec < 100)
				pad = "  ";
			else if (sec < 1000)
				pad = " ";
			else
				pad = "";
			
			/* calculate 10 times the percentage of total */
			percent10 = (time_val * 1000) / total_time;
			
			/* build and return the output string */
			return (neg?"-":"") + pad + sec + "." + ((ms % 1000) / 100) + ((ms % 100) / 10) + (ms % 10) + "sec" + " (" + percent10 / 10 + "." + percent10 % 10 + "%)";
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Produce a human readable dump of the grammar. 
		/// </summary>
		public static void  dump_grammar()
		{
			System.Console.Error.WriteLine("===== Terminals =====");
			 for (int tidx = 0, cnt = 0; tidx < terminal.number(); tidx++, cnt++)
			{
				System.Console.Error.Write("[" + tidx + "]" + terminal.find(tidx).name_Renamed_Method() + " ");
				if ((cnt + 1) % 5 == 0)
					System.Console.Error.WriteLine();
			}
			System.Console.Error.WriteLine();
			System.Console.Error.WriteLine();
			
			System.Console.Error.WriteLine("===== Non terminals =====");
			 for (int nidx = 0, cnt = 0; nidx < non_terminal.number(); nidx++, cnt++)
			{
				System.Console.Error.Write("[" + nidx + "]" + non_terminal.find(nidx).name_Renamed_Method() + " ");
				if ((cnt + 1) % 5 == 0)
					System.Console.Error.WriteLine();
			}
			System.Console.Error.WriteLine();
			System.Console.Error.WriteLine();
			
			
			System.Console.Error.WriteLine("===== Productions =====");
			 for (int pidx = 0; pidx < production.number(); pidx++)
			{
				production prod = production.find(pidx);
				System.Console.Error.Write("[" + pidx + "] " + prod.lhs().the_symbol().name_Renamed_Method() + " ::= ");
				 for (int i = 0; i < prod.rhs_length(); i++)
					if (prod.rhs(i).is_action())
						System.Console.Error.Write("{action} ");
					else
						System.Console.Error.Write(((symbol_part) prod.rhs(i)).the_symbol().name_Renamed_Method() + " ");
				System.Console.Error.WriteLine();
			}
			System.Console.Error.WriteLine();
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Produce a (semi-) human readable dump of the complete viable prefix 
		/// recognition state machine. 
		/// </summary>
		public static void  dump_machine()
		{
			lalr_state[] ordered = new lalr_state[lalr_state.number()];
			
			/* put the states in sorted order for a nicer display */
			//UPGRADE_TODO: method 'java.util.Enumeration.hasMoreElements' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationhasMoreElements"'
			 for (System.Collections.IEnumerator s = lalr_state.all(); s.MoveNext(); )
			{
				//UPGRADE_TODO: method 'java.util.Enumeration.nextElement' was converted to ' ' which has a different behavior. 'ms-help://MS.VSCC/commoner/redir/redirect.htm?keyword="jlca1073_javautilEnumerationnextElement"'
				lalr_state st = (lalr_state) s.Current;
				ordered[st.index()] = st;
			}
			
			System.Console.Error.WriteLine("===== Viable Prefix Recognizer =====");
			 for (int i = 0; i < lalr_state.number(); i++)
			{
				if (ordered[i] == start_state)
					System.Console.Error.Write("START ");
				System.Console.Error.WriteLine(ordered[i]);
				System.Console.Error.WriteLine("-------------------");
			}
		}
		
		/*. . . . . . . . . . . . . . . . . . . . . . . . . . . . . .*/
		
		/// <summary>Produce a (semi-) human readable dumps of the parse tables 
		/// </summary>
		public static void  dump_tables()
		{
			System.Console.Error.WriteLine(action_table);
			System.Console.Error.WriteLine(reduce_table);
		}
		
		/*-----------------------------------------------------------*/
	}
}