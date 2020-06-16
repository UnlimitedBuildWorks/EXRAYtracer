using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;
using static EXRAY.rayh;
using static EXRAY.raysh;

namespace EXRAY
{
    public class ray0 {
        public static void terminate(int status)
        {
            throw new ApplicationException();
        }

        public static void set_slabel()
        {
            slabel[P_EOF] = " ";
            slabel[P_COLOR] = "color";
            slabel[P_RGB] = "rgb";
            slabel[P_FD] = "fd";
            slabel[P_FH] = "fh";
            slabel[P_HM] = "hm";
            slabel[P_HC] = "hc";
            slabel[P_FS1] = "fs1";
            slabel[P_FS2] = "fs2";
            slabel[P_T0] = "t0";
            slabel[P_N] = "n";
            slabel[P_INIT] = "init";
            slabel[P_PRIMITIVE] = "primitive";
            slabel[P_KIND] = "kind";
            slabel[P_PABC] = "pabc";
            slabel[P_RBPH] = "rbph";
            slabel[P_OXYZ] = "oxyz";
            slabel[P_SG] = "sg";
            slabel[P_SCM] = "scm";
            slabel[P_LIGHT] = "light";
            slabel[P_TYPE] = "type";
            slabel[P_MODULE] = "module";
            slabel[P_WORLD] = "world";
            slabel[P_MAPPING] = "mapping";
            slabel[P_DEFINE] = "define";
            slabel[P_INCLUDE] = "include";
            slabel[P_SCALE] = "scale";
            slabel[P_PREFIX] = "prefix";
            slabel[P_UNPREFIX] = "unprefix";
            slabel[P_SOURCE] = "source";
            slabel[P_UNDEF] = "undef";
            use_slabel = P_END;
            use_label = 0;
            use_value = 0;
        }
        //#define ADD_LABEL 100
        private const int ADD_LABEL = 100;

        public static void more_label()
        {
            uint no;
            uint i;
            //REGISTER char(*sub_label)[LEN_LABEL + 1];
            //string[] sub_label;
            //uint[] sub_macro;

            /* Extending of label[] */
            Debug.Log("Extending of label[].\n");
            no = max_label + ADD_LABEL;
            //if ((sub_label = (char(*)[LEN_LABEL + 1])malloc(no * (LEN_LABEL + 1)
            //                        * sizeof(char))) == NULL) {
            //    fprintf(stderr, "*** No memory for allocation. ***\n");
            //    terminate(1);
            //}
            //for (i = 0; i < use_label; i++)
            //    strcpy(sub_label[i], label[i]);
            //free((char*)label);
            //label = sub_label;
            System.Array.Resize(ref label, (int)no);

            /* Extending of macro[] */
            no = max_label + BASE_LABEL + ADD_LABEL;
            //if ((sub_macro = (POINTER*)malloc(no * sizeof(POINTER))) == NULL)
            //{
            //    fprintf(stderr, "*** No memory for allocation. ***\n");
            //    terminate(1);
            //}
            //for (i = 0; i < max_label + BASE_LABEL; i++)
            //    sub_macro[i] = macro[i];
            //free((char*)macro);
            //macro = sub_macro;
            System.Array.Resize(ref macro, (int)no);
            for (i = max_label + BASE_LABEL; i < max_label + BASE_LABEL + ADD_LABEL; i++) macro[i] = i;
            max_label += ADD_LABEL;
        }

        //#define ADD_VALUE 100
        private const int ADD_VALUE = 100;
        public static void more_value()
        {
            uint no;
            uint i;
            //REGISTER double* sub_value;

            /* Extending of value[] */
            Debug.Log("Extending of value[].\n");
            no = max_value + ADD_VALUE;
            //if ((sub_value = (double*)malloc(no * sizeof(double))) == NULL)
            //{
            //    fprintf(stderr, "*** No memory for allocation. ***\n");
            //    terminate(1);
            //}
            //for (i = 0; i < use_value; i++)
            //    sub_value[i] = value[i];
            //free((char*)value);
            //value = sub_value;
            System.Array.Resize(ref value, (int)no);
            max_value += ADD_VALUE;
        }

        public static char upper(char c)
        {
            //if (islower(c)) return (toupper(c));
            //return (c);
            return System.Char.ToUpper(c);
        }

        /* string searching program */
        public static int instr(string str, char c)
        {
            //int i;
            //
            //for (i = 1; *str != '\0'; i++)
            //    if (*(str++) == c)  /* found */
            //        return (i);
            /* not found */
            //return (0);

            return (str.IndexOf(c) + 1);
        }

        /* separater check program */
        public static int issepa(int c)
        {
            //if (isalnum(c))     /* alphabet or numerical charactor */
            //    return (0);
            if (System.Char.IsLetterOrDigit((char)c)) return (0);
            /* check 1st separater or EOF */
            if (cntrl((char)c) || instr(" ,/*", (char)c) > 0 || c == EOF)
                return (1);
            /* check 2nd separater */
            if (instr("{}()#", (char)c) > 0)
                return (2);
            return (0);
        }

        /* make temporary file program */
        public static tag_TEMP stempfile(string s_file)
        {
            StreamReader fp;
            tag_TEMP tp;
            int c;
            int i;
            uint no;
            char[] temp = new char[LEN_TEMP + 1];
            double tvalue;

            /* open files */
            //if ((fp = fopen(s_file, READ_TEXT)) == NULL)
            //{
            //    fprintf(stderr, "*** Can't open %s file. ***\n",s_file);
            //    terminate(1);
            //}
            using (fp = new StreamReader(s_file))
            {

                tp = temp_open();       /* open temporary file */

                while ((c = fp.Read()) != EOF)
                {
                    if (0 == issepa((char)c))
                    {
                        temp[0] = (char)c;
                        for (i = 1; 0 == issepa(c = fp.Read());)
                            if (i < LEN_TEMP)
                                temp[i++] = (char)c;
                            else
                            {
                                temp[i] = (char)'\0';
                                Debug.LogError(string.Format("*** Too long label or value({0}) in '{1}'. ***\n", C_String(temp), s_file));
                                //terminate(1);
                                throw new ApplicationException();
                            }
                        temp[i++] = '\0';   /* bottom of temp[] */
                        if (Char.IsDigit(temp[0]) || 0 != instr("+-.", temp[0]))
                        {
                            /* VALUE */
                            //if (sscanf(temp, "%lf", &tvalue) != 1)
                            //{
                            //    fprintf(stderr, "*** Error value(%s) in '%s'. ***\n", temp, s_file);
                            //    terminate(1);
                            //}
                            string sss = C_String(temp);
                            tvalue = Convert.ToDouble(sss);
                            //tvalue = Convert.ToDouble(new C_String(temp));
                            if (use_value >= max_value)
                            {
                                more_value();
                            }
                            no = use_value;
                            for (i = 0; i < use_value; i++)
                                if (value[i] == tvalue)
                                {
                                    no = (uint)i;
                                    break;
                                }
                            if (no == use_value)
                                value[use_value++] = tvalue;
                            wword(tp, no + BASE_VALUE);
                        }
                        else
                        {
                            /* LABEL */
                            temp[LEN_LABEL] = '\0';
                            no = BASE_LABEL;
                            for (i = 0; i < use_slabel; i++)
                                if (C_String(temp) == slabel[i])
                                {
                                    no = (uint)i;
                                    break;
                                }
                            if (no >= BASE_LABEL)
                            {
                                if (use_label >= max_label)
                                {
                                    more_label();
                                }
                                no = use_label;
                                for (i = 0; i < use_label; i++)
                                    if (C_String(temp) == label[i])
                                    {
                                        no = (uint)i;
                                        break;
                                    }
                                if (no == use_label)
                                    //strcpy(label[use_label++], temp);
                                    label[use_label++] = C_String(temp);
                                /* copy string */
                                no += BASE_LABEL;
                            }
                            wword(tp, no);
                        }
                        if (c == EOF)
                            break;
                    }
                    else            /* SEPARATER */
                        if (c == '/')
                        if ((c = fp.Peek()) != '*')
                        {
                            //ungetc(c, fp);
                            c = '/';
                        }
                        else
                        {
                            c = fp.Read();
                            while (true)
                            {
                                c = fp.Read();
                            LOOP:
                                if (c == EOF) break;
                                if (c != '*') continue;
                                if ((c = fp.Read()) == '/') break;
                                goto LOOP;
                            }
                        }
                    if (issepa(c) == 2) /* Special separater */
                        wword(tp, (uint)(c + BASE_SEPA));

                }
                /* source file ended */
                wword(tp, (uint)P_EOF);

                /* close files */
                //fclose(fp);
            }

            /* rewind */
            temp_rewind(tp);

            return (tp);
        }

        public static tag_TEMP tempfile(string s_file)
        {
            tag_TEMP tp;
            uint i;

            /* initialize */
            for (i = 0; i < max_label + BASE_LABEL; i++)
                macro[i] = i;
            tp = temp_open();
            maketempfile(tp, s_file, (int)(-1));
            wword(tp, (uint)P_EOF);
            /* Rewind */
            temp_rewind(tp);
            return (tp);
        }

        /* Macro pre-processor */
        public static void maketempfile(tag_TEMP tp, string s_file, int n)
        {
            tag_TEMP tps;
            uint a, b;

            n++;

            tps = stempfile(s_file);

            while (!chkeof(a = rword(tps)))
            {
                if (chkvalue(a))
                {
                //WRITE:
                    wword(tp, a);
                    continue;
                }
                if (chklabel(a))
                {
                    a = macro[a];
                //goto WRITE;
                //WRITE:
                    wword(tp, a);
                    continue;

                }
                if (a != BASE_SEPA + '#')
                {
                //goto WRITE;
                //WRITE:
                    wword(tp, a);
                    continue;
                }
                switch (a = rword(tps))
                {
                    case P_DEFINE:
                        a = rword(tps);
                        b = rword(tps);
                        if (!chklabel(a) || !(chklabel(b) || chkvalue(b)))
                            goto serror;
                        macro[a] = b;
                        continue;

                    case P_UNDEF:
                        a = rword(tps);
                        if (!chklabel(a))
                            goto serror;
                        macro[a] = a;
                        continue;

                    case P_INCLUDE:
                        a = rword(tps);
                        if (!chklabel(a))
                            goto serror;
                        maketempfile(tp, dataPath + "/" + poilabel(a), n);
                        continue;

                    default:
                        goto serror;
                }
            }
            temp_unlink(ref tps);       /* remove the temporary file */
            return;

        serror:             /* error happened */
            Debug.LogError(String.Format("*** Syntax error in '{0}' file. ***\n", s_file));
            //terminate(1);
            throw new ApplicationException();
        }

        /* Standard Temporary file Input/Output functions */

        /* get more core program */
        private static uint[] more_core()
        {
            uint[] core;

            //if ((core = (POINTER*)malloc(TEMP_CORE_SIZE * sizeof(POINTER))) == NULL)
            //{
            //    fprintf(stderr, "*** No memory for allocation. ***\n");
            //    terminate(1);
            //}
            core = new uint[TEMP_CORE_SIZE];
            return (core);
        }

        /* Open temporary file */
        public static tag_TEMP temp_open()
        {
            tag_TEMP tp;

            //if ((tp = (TEMP*)malloc(sizeof(TEMP))) == NULL)
            //{
            //    fprintf(stderr, "*** No memory for allocation. ***\n");
            //    terminate(1);
            //}
            tp = new tag_TEMP();
            tp.core[0] = tp.l_position = more_core(); tp.position = 0;
            tp.max_core = 1;
            tp.now_core = 0;
            tp.counter = TEMP_CORE_SIZE;
            return (tp);
        }

        /* Remove the temporary file */
        public static void temp_unlink(ref tag_TEMP tp)
        {
            uint i;

            for (i = 0; i < tp.max_core; i++)
                //free((char*)tp->core[i]);
                tp.core[i] = null;
            //free((char*)tp);
            tp = null;
        }

        /* Rewind 'tp' */
        public static void temp_rewind(tag_TEMP tp)
        {
            tp.l_position = tp.core[tp.now_core = 0]; tp.position = 0;
            tp.counter = TEMP_CORE_SIZE;
        }

        /* Tell the tp */
        public static void temp_tell(tag_TEMP tp, tag_TEMP_SUB temp)
        {
            temp.position = tp.position;
            temp.l_position = tp.l_position;
            temp.now_core = tp.now_core;
            temp.counter = tp.counter;
        }

        /* Seek the tp */
        public static void temp_seek(tag_TEMP tp, tag_TEMP_SUB temp)
        {
            tp.position = temp.position;
            tp.l_position = temp.l_position;
            tp.now_core = temp.now_core;
            tp.counter = temp.counter;
        }

        /* Flush and Read one word */
        public static uint flush_rword(tag_TEMP tp)
        {
            uint data;

            data = tp.l_position[tp.position];
            if (++tp.now_core >= tp.max_core)
            {
                Debug.LogError("*** Inside error(rword()). ***\n");
                //terminate(1);
                throw new ApplicationException();
            }
            //tp->position = tp->core[tp->now_core];
            tp.l_position = tp.core[tp.now_core];
            tp.position = 0;
            tp.counter = TEMP_CORE_SIZE;
            return (data);
        }

        /* Unget one word */
        public static void temp_ungetc(tag_TEMP tp)
        {
            --tp.position;
            if (++tp.counter > TEMP_CORE_SIZE)
            {
                if (tp.now_core-- == 0)
                {
                    Debug.LogError("*** Inside error(temp_ungetc()). ***\n");
                    //terminate(1);
                    throw new ApplicationException();
                }
                tp.l_position = tp.core[tp.now_core]; tp.position = TEMP_CORE_SIZE - 1;
                tp.counter = 1;
            }
        }

        /* Flush and Write one word */
        public static void flush_wword(tag_TEMP tp, uint data)
        {
            //*(tp->position) = data;
            tp.l_position[tp.position] = data;
            if (tp.max_core >= TEMP_MAX_CORE)
            {
                Debug.LogError("*** Too long temporary file. ***\n");
                //terminate(1);
                throw new ApplicationException();
            }
            //tp->position = tp->core[tp->max_core++] = more_core();
            tp.l_position = tp.core[tp.max_core++] = more_core();
            tp.position = 0;
            ++tp.now_core;
            tp.counter = TEMP_CORE_SIZE;
        }
    }
}
