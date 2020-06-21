using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System;
using UnityEngine;
using static EXRAY.rayh;
using static EXRAY.raysh;
using static EXRAY.ray0;
using static EXRAY.ray2;

namespace EXRAY
{
    public class ray1
    {
        /* Read binary-tree data from file(-r option). */
        public static bool read_btree(int argc, string[] argv)
        {
            int i;
            uint j;
            StreamReader fp;

            for (i = 1; i < argc - 1; i++)
                if (argv[i] == "-r")
                {
                    //if ((fp = fopen(argv[i + 1], READ_TEXT)) == NULL)
                    //{
                    //	fprintf(stderr, "*** Can't open %s file. ***\n", argv[i + 1]);
                    //	terminate(1);
                    //}
                    using (fp = new StreamReader(argv[i + 1]))
                    {
                        //if (fscanf(fp, "%u", &use_btree) != 1)
                        //{
                        //error:
                        //	fprintf(stderr, "*** Format error in '%s' file. ***", argv[i + 1]);
                        //	terminate(1);
                        //}
                        string s = fp.ReadLine();
                        use_btree = Convert.ToUInt32(s);
                        for (j = 0; j < use_btree; j++)
                        {
                            //if (fscanf(fp, "%u", &btree[j]) != 1)
                            //{
                            //	goto error;
                            //}
                            s = fp.ReadLine();
                            btree[j] = Convert.ToUInt32(s);
                        }
                        //fclose(fp);
                        return (true);
                    }
                }
            return (false);
        }

        /* Write binary-tree data to file(-w option). */
        public static void write_btree(int argc, string[] argv)
        {
            uint i;
            StreamWriter fp;

            for (i = 0; i < argc - 1; i++)
            {
                if (argv[i] == "-w")
                {
                    //if ((fp = fopen(argv[i + 1], WRITE_TEXT)) == NULL)
                    //{
                    //	fprintf(stderr, "*** Can't open '%s' file. ***\n", argv[i + 1]);
                    //	terminate(1);
                    //}
                    using (fp = new StreamWriter(argv[i + 1]))
                    {
                        //fprintf(fp, "%u ", use_btree);
                        fp.WriteLine(Convert.ToUInt32(use_btree));
                        for (i = 0; i < use_btree; i++)
                        {
                            //fprintf(fp, "%u ", btree[i]);
                            fp.WriteLine(Convert.ToUInt32(btree[i]));
                        }
                        //fclose(fp);
                        return;
                    }
                }
            }
        }

        /* Input/Output process main program */
        public static void io_main(int ScreenX, int ScreenY, int argc, string[] argv)
        {
            StreamReader fp;
            string fname = "";
            int i;
            uint ii;
            tag_prim dmy;
            //char nscf[50], npdf[50], nmdf[50];
            string nscf, npdf, nmdf;
            uint jj, kk, ll, mm, nn, oo;

            set_slabel();       /* Set slabel[] */

            for (i = 1; i < argc; i++)  /* Argument check */
                if (argv[i][0] != '-')
                {
                    fname = argv[i];
                    break;
                }

            if (i >= argc)
            {
                Debug.LogErrorFormat("Usage : {0} initialize_file.ray [-c] [-ct] [-w file_name] [-r file_name]\n", argv[0]);
                Debug.LogError("        '-c'   Data check only.(no ray-tracing)\n");
                Debug.LogError("        '-ct'  Continue the last ray-tracing.\n");
                Debug.LogError("        '-w'   Write the Binary-Tree data to file.\n");
                Debug.LogError("        '-r'   Read the Binary-Tree data from file.\n");
                //terminate(1);
                throw new ApplicationException();
            }


            //if ((fp = fopen(fname, READ_TEXT)) == NULL) {
            //	fprintf(stderr, "*** Can't open '%s' file. ***\n", fname);
            //	terminate(1);
            //}
            tag_view view = views[0];
            using (fp = new StreamReader(fname))
            {
                /* read datas from initialize file */
                //if (fscanf(fp, "%*s %s %*s %s %*s %s %*s %s", nscf, npdf, nmdf, view.object) != 4) {
                //	fprintf(stderr, "*** Initial data error (FILE NAME) in '%s'. ***\n", fname);
                //	terminate(1);
                //}
                char[] sep = { ' ', '\t' };
                string[] vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                nscf = dataPath + "/" + vals[1];
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                npdf = dataPath + "/" + vals[1];
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                nmdf = dataPath + "/" + vals[1];
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                view.Object = dataPath + "/" + vals[1];

                //if (fscanf(fp, "%*s %d %*s %d %*s %d %*s %d %*s %d %*s %d %*s %d %*s %d"
                //	   , &max_label, &max_value, &max_scm, &max_pd, &max_prim
                //	   , &max_light, &max_atable, &max_btree) != 8) {
                //	fprintf(stderr, "*** Initial data error (SIZE OF STRUCTURES OR ARRAYS) in '%s'. ***\n", fname);
                //	terminate(1);
                //}
                fp.ReadLine(); // Ignore null line.
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                max_label = Convert.ToUInt32(vals[1]);
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                max_value = Convert.ToUInt32(vals[1]);
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                max_scm = Convert.ToUInt32(vals[1]);
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                max_pd = Convert.ToUInt32(vals[1]);
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                max_prim = Convert.ToUInt32(vals[1]);
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                max_light = Convert.ToUInt32(vals[1]);
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                max_atable = Convert.ToUInt32(vals[1]);
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                max_btree = Convert.ToUInt32(vals[1]);

                //if (fscanf(fp, "%*s %lf %lf %*s %lf %*s %d %d %*s %d %*s %lf %*s %d"
                //	   , &view.scrx, &view.scry, &view.scrl, &view.pxlx, &view.pxly
                //	   , &view.anti, &view.anti_aliasing_factor, &view.shadow) != 8) {
                //	fprintf(stderr, "*** Initial data error (AND SO ON) in '%s'. ***\n", fname);
                //	terminate(1);
                //}
                fp.ReadLine(); // Ignore null line.
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                view.scrx = Convert.ToDouble(vals[1]);
                view.scry = Convert.ToDouble(vals[2]);
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                view.scrl = Convert.ToDouble(vals[1]);
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                view.pxlx = Convert.ToInt32(vals[1]); view.pxlx = ScreenX;
                view.pxly = Convert.ToInt32(vals[2]); view.pxly = ScreenY;
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                view.anti = Convert.ToInt32(vals[1]);
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                view.anti_aliasing_factor = Convert.ToDouble(vals[1]);
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                view.shadow = Convert.ToInt32(vals[1]) != 0;


                //if (fscanf(fp, "%*s %d %*s %d %*s %d %*s %lf %lf %lf %*s %lf %lf %lf %*s %d %*s %d"
                //	   , &view.nmin, &view.nmax, &view.pmax
                //	   , &view.maxx, &view.maxy, &view.maxz
                //	   , &view.minx, &view.miny, &view.minz
                //	   , &view.method, &view.trans) != 11) {
                //	fprintf(stderr, "*** Initial data error (AND SO ON) in '%s'. ***\n", fname);
                //	terminate(1);
                //}
                fp.ReadLine(); // Ignore null line.
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                view.nmin = Convert.ToInt32(vals[1]);
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                view.nmax = Convert.ToInt32(vals[1]);
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                view.pmax = Convert.ToInt32(vals[1]);
                fp.ReadLine();  // Ignore null line.
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                view.maxx = Convert.ToDouble(vals[1]);
                view.maxy = Convert.ToDouble(vals[2]);
                view.maxz = Convert.ToDouble(vals[3]);
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                view.minx = Convert.ToDouble(vals[1]);
                view.miny = Convert.ToDouble(vals[2]);
                view.minz = Convert.ToDouble(vals[3]);
                fp.ReadLine();  // Ignore null line.
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                view.method = Convert.ToInt32(vals[1]) != 0;
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                view.trans = Convert.ToInt32(vals[1]) != 0;

                //if (fscanf(fp, "%*s %d %*s %d %*s %lf %*s %lf"
                //	   , &view.branch_level, &view.depth_level
                //	   , &view.limit_power, &view.ts_factor) != 4) {
                //	fprintf(stderr, "*** Initial data error (AND SO ON) in '%s'. ***\n", fname);
                //	terminate(1);
                //}
                fp.ReadLine(); // Ignore null line.
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                view.branch_level = Convert.ToInt32(vals[1]);
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                view.depth_level = Convert.ToInt32(vals[1]);
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                view.limit_power = Convert.ToDouble(vals[1]);
                vals = fp.ReadLine().Split(sep, StringSplitOptions.RemoveEmptyEntries);
                view.ts_factor = Convert.ToDouble(vals[1]);
            }

            /* value check */
            if (view.scrx <= 0 || view.scry <= 0 || view.pxlx < 1 || view.pxly < 1
                || view.nmin <= 0 || view.nmax <= 0 || view.pmax <= 0
                || view.maxx <= view.minx || view.maxy <= view.miny || view.maxz <= view.minz
                || max_label < 1 || max_value < 1 || max_scm < 1 || max_pd < 1 || max_prim < 1
                || max_light < 1 || max_atable < 1 || max_btree < 1
                || view.branch_level < 0 || view.depth_level < 0 || !chk01(view.limit_power) || !chk01(view.ts_factor) || view.anti < 0 || !chk01(view.anti_aliasing_factor))
            {
                Debug.LogError(String.Format("*** Initial data error (Illegal value) in '{0}'. ***\n", fname));
                //terminate(1);
                throw new ApplicationException();
            }

            /* set initial parameter k1-k4 */
            if (0 != view.anti)
            {       /* anti-aliasing */
                view.k1 = view.scrx / view.pxlx;
                view.k2 = -0.5 * ((double)view.pxlx);
                view.k3 = -view.scry / view.pxly;
                view.k4 = -0.5 * ((double)view.pxly);
            }
            else
            {            /* non anti-aliasing */
                view.k1 = view.scrx / view.pxlx;
                view.k2 = 0.5 * (1.0 - ((double)view.pxlx));
                view.k3 = -view.scry / view.pxly;
                view.k4 = 0.5 * (1.0 - ((double)view.pxly));
            }

            /* memory allocation of structures */
            //scm = (struct tag_scm  *)malloc(max_scm* sizeof(struct tag_scm));
            scm = new tag_scm[max_scm];
            for (i = 0; i < max_scm; i++) scm[i] = new tag_scm();
            uint iii = max_scm; Debug.Log(iii);
            //pd  =   (struct tag_pd *)malloc(max_pd* sizeof(struct tag_pd));
            pd = new tag_pd[max_pd];
            for (i = 0; i < max_pd; i++) pd[i] = new tag_pd();
            //prim = (struct tag_prim *)malloc(max_prim* sizeof(struct tag_prim));
            prim = new tag_prim[max_prim];
            for (i = 0; i < max_prim; i++) prim[i] = new tag_prim();
            //light = (struct tag_light *)malloc(max_light* sizeof(struct tag_light));
            light = new tag_light[max_light];
            for (i = 0; i < max_light; i++) light[i] = new tag_light();
            /* memory allocation of arrays */
            //label  = (char (*)[LEN_LABEL + 1])malloc(max_label* (LEN_LABEL + 1) * sizeof(char));
            label = new string[max_label];
            //value  = (double*) malloc(max_value* sizeof(double));
            value = new double[max_value];
            //atable = (POINTER (*)[2])malloc(max_atable* 2 * sizeof(POINTER));
            atable = new uint[max_atable, 2];
            //btree  = (POINTER*) malloc(max_btree* sizeof(POINTER));
            btree = new uint[max_btree];
            //macro  = (POINTER*) malloc((max_label + BASE_LABEL) * sizeof(POINTER));
            macro = new uint[max_label + BASE_LABEL];

            /* check memory allocation is normal ? */
            //if (scm == NULL || pd == NULL || prim == NULL || light == NULL
            //	|| label == NULL || value == NULL || atable == NULL
            //	|| btree == NULL || macro == NULL) {
            //	fprintf(stderr, "*** No memory for allocation. ***\n");
            //	terminate(1);
            //}

            /* process Surface Color File(SCF) */
            cvtscm(nscf);
            Debug.Log("Completed : cvtscm()\n");
            Debug.Log(String.Format("use_scm {0:N6} max_scm {1:N6} free_scm {2:N6}\n\n", use_scm, max_scm, max_scm - use_scm));

            /* process Primitive Data FIle(PDF) */
            cvtpd(npdf);
            Debug.Log("Completed : cvtpd()\n");
            Debug.Log(string.Format("use_pd {0:N6}  max_pd {1:N6} free_pd {2:N6}\n\n", use_pd, max_pd, max_pd - use_pd));

            /* process Module Data FIle(MDF) */
            cvtmd(nmdf);
            Debug.Log("Completed : cvtmd()\n");
            Debug.Log(String.Format("use_prim {0:N6}  max_prim {1:N6} free_prim {2:N6}\n", use_prim, max_prim, max_prim - use_prim));
            Debug.Log(String.Format("use_atable {0:N6}  max_atable {1:N6} free_atable {2:N6}\n", use_atable, max_atable, max_atable - use_atable));
            Debug.Log(String.Format("use_light {0:N6}  max_light {1:N6} free_light {2:N6}\n", use_light, max_light, max_light - use_light));
            Debug.Log(String.Format("transparent and-table {0:N6}\n\n", use_trans));

            /* '-c' option is check file format only. */
            for (i = 1; i < argc; i++)
                if (argv[i] == "-c")
                {
                    Debug.Log("\nFile check ok.\nNormal Terminated.\n");
                    //terminate(1);
                    throw new ApplicationException();
                }

            /* make binary tree(btree[]) */
            if (!read_btree(argc, argv))
            {
                if (view.method)
                {
                    mkbtree();
                    Debug.Log("Completed : mkbtree()\n");
                    Debug.LogFormat("use_btree {0:N6}  max_btree {1:N6} free_btree {2:N6}\n", use_btree, max_btree, max_btree - use_btree);
                    Debug.Log(String.Format("the number of outsider primitives {0:N}.\n\n", use_outprim));
                    write_btree(argc, argv);
                }
                else
                {
                    btree[0] = btree[1] = 0;
                }
            }

            /* free the memory */
            //free((char*) pd);
            //free((char*) label);
            //free((char*) value);
            //free((char*) macro);
        }

        private const int ADD_SCM = 30;
        public static void more_scm()
        {
            //uint no;
            //uint i;
            //tag_scm[] sub_scm;

            /* Extending of scm[] */
            Debug.Log("Extending of scm[].\n");
            //no = use_scm + ADD_SCM;
            //if ((sub_scm = (struct tag_scm *)malloc(no* sizeof (struct tag_scm))) == NULL) {
            //	fprintf(stderr, "*** No memory for allocation. ***\n");
            //	terminate(1);
            //};
            //sub_scm = new tag_scm[no];
            //for (i = 0; i < use_scm; i++)
            //    scmcpy(sub_scm[i], scm[i]);
            //free((char*)scm);
            //scm = sub_scm;
            uint i = max_scm + 1;
            max_scm += ADD_SCM;
            Array.Resize(ref scm, (int)max_scm);
            for (; i < max_scm; i++) scm[i] = new tag_scm();
        }

        /* Convert scm data */
        public static void cvtscm(string fname)
        {
            tag_TEMP tp;
            tag_scm iiscm = new tag_scm();
            //0, 0, 1.0, 1.0, 1.0, 0.8, 1.0, 0, 4.0, 0.2, 0.2, 100, 0, 0, 1.5, 0
            iiscm.label = 0;
            iiscm.type = 0;
            iiscm.r = iiscm.g = iiscm.b = 1.0;
            iiscm.fd = 0.8;
            iiscm.fh = 1.0;
            iiscm.hm = 0;
            iiscm.hc = 4.0;
            iiscm.fs1 = iiscm.fs2 = 0.2;
            iiscm.ka = 100;
            iiscm.kb = iiscm.kc = 0;
            iiscm.n = 1.5;
            iiscm.source_no = 0;
            tag_scm tscm = new tag_scm();
            tag_scm iscm = new tag_scm();
            uint no = 0;
            uint i;
            uint j;
            bool flag;
            uint a, b, c;
            double t0;
            double[] work = new double[21];
            double w;
            double[] param = new double[3];
            double[] dmyparam = {
                1.0, 1.0, 1.0
            };              /* This is dummy data. */

            iiscm.mapping.mapping_no = 0;
            iiscm.mapping.rb = iiscm.mapping.rp = iiscm.mapping.rh = 0;
            iiscm.mapping.ox = iiscm.mapping.oy = iiscm.mapping.oz = 0;
            iiscm.mapping.sclx = iiscm.mapping.scly = iiscm.mapping.sclz = 1.0;
            use_scm = 0;
            scmcpy(iscm, iiscm);

            tp = tempfile(fname);   /* make temporary file */

            while (!chkeof(i = rword(tp)))
            {
                scmcpy(tscm, iscm);
                flag = false;
                if (i == P_COLOR) flag = true;
                else if (i != P_INIT) goto serror;
                no = P_INIT;
                if (flag)
                {
                    no = rword(tp);
                    if (!chklabel(no) || chkeof(no)) goto serror;
                    /* check multi-definition of SCM */
                    for (i = 0; i < use_scm; i++)
                    {
                        if (scm[i].label == no)
                        {
                            Debug.LogError(String.Format("*** Multi-define SCM({0}) in '{1}'. ***\n", poilabel(no), fname));
                            //terminate(1);
                            throw new ApplicationException();
                        }
                    }
                    tscm.label = no;
                }
                a = rword(tp);
                if (a != BASE_SEPA + '{') goto serror;
                while ((a = rword(tp)) != BASE_SEPA + '}')
                {
                    switch (a)
                    {
                        case P_TYPE:
                            a = rword(tp);
                            if (!chkvalue(a)) goto serror;
                            tscm.type = (int)value[a - BASE_VALUE];
                            break;

                        case P_RGB:
                            a = rword(tp);
                            b = rword(tp);
                            c = rword(tp);
                            if (!chkvalue(a) || !chkvalue(b) || !chkvalue(c))
                                goto serror;
                            tscm.r = value[a - BASE_VALUE];
                            tscm.g = value[b - BASE_VALUE];
                            tscm.b = value[c - BASE_VALUE];
                            break;

                        case P_FD:
                            a = rword(tp);
                            if (!chkvalue(a)) goto serror;
                            tscm.fd = value[a - BASE_VALUE];
                            break;

                        case P_FH:
                            a = rword(tp);
                            if (!chkvalue(a)) goto serror;
                            tscm.fh = value[a - BASE_VALUE];
                            break;

                        case P_HM:
                            a = rword(tp);
                            if (!chkvalue(a)) goto serror;
                            tscm.hm = (int)value[a - BASE_VALUE];
                            break;

                        case P_HC:
                            a = rword(tp);
                            if (!chkvalue(a)) goto serror;
                            tscm.hc = value[a - BASE_VALUE];
                            break;

                        case P_FS1:
                            a = rword(tp);
                            if (!chkvalue(a)) goto serror;
                            tscm.fs1 = value[a - BASE_VALUE];
                            break;

                        case P_FS2:
                            a = rword(tp);
                            if (!chkvalue(a)) goto serror;
                            tscm.fs2 = value[a - BASE_VALUE];
                            break;

                        case P_T0:
                            a = rword(tp);
                            if (!chkvalue(a)) goto serror;
                            tscm.ka = value[a - BASE_VALUE];
                            break;

                        case P_N:
                            a = rword(tp);
                            if (!chkvalue(a)) goto serror;
                            tscm.n = value[a - BASE_VALUE];
                            break;

                        case P_MAPPING:
                            a = rword(tp);
                            if (!chkvalue(a)) goto serror;
                            tscm.mapping.mapping_no = (int)value[a - BASE_VALUE];
                            if (tscm.mapping.mapping_no < 0
                                || tscm.mapping.mapping_no > MAX_MAPPING_NO)
                                goto verror;
                            break;

                        case P_SOURCE:
                            a = rword(tp);
                            if (!chkvalue(a)) goto serror;
                            tscm.source_no = (int)value[a - BASE_VALUE];
                            if (tscm.source_no < 0 || tscm.source_no > MAX_SOURCE_NO)
                                goto verror;
                            switch (tscm.source_no)
                            {
                                case 0:
                                    break;

                                case 1:
                                case 2:
                                    a = rword(tp);
                                    if (!chklabel(a)) goto serror;
                                    //if ((tscm.mapping.fp = fopen(poilabel(a), READ_BINARY)) == NULL)
                                    //{
                                    //    fprintf(stderr, "*** Can't open '%s' file in SCM(%s) in '%s'. ***\n"
                                    //        , poilabel(a), poilabel(no), fname);
                                    //    terminate(1);
                                    //}
                                    string tex_fname = dataPath + "/" + poilabel(a);
                                    using (tscm.mapping.fp = new FileStream(tex_fname, FileMode.Open, FileAccess.Read))
                                    {
                                        tscm.mapping.fx = tscm.mapping.fp.ReadByte() * 256;
                                        /* file size X */
                                        tscm.mapping.fx += tscm.mapping.fp.ReadByte();
                                        tscm.mapping.fy = tscm.mapping.fp.ReadByte() * 256;
                                        /* file size Y */
                                        tscm.mapping.fy += tscm.mapping.fp.ReadByte();

                                        if (tscm.source_no == 1)
                                        {
                                            /* Copy file to memory area */
                                            j = (uint)(tscm.mapping.fx * tscm.mapping.fy * 3);
                                            //if ((tscm.mapping.mem = malloc(j * sizeof(char))) == NULL)
                                            //{
                                            //    fprintf(stderr, "*** No memory for allocation. ***\n");
                                            //    terminate(1);
                                            //}
                                            tscm.mapping.mem = new byte[j];
                                            for (i = 0; i < j; i++)
                                            {
                                                tscm.mapping.mem[i] = (byte)tscm.mapping.fp.ReadByte();
                                                //if (feof(tscm.mapping.fp))
                                                if (tscm.mapping.fp.Position > tscm.mapping.fp.Length)
                                                {
                                                    Debug.LogError(String.Format("*** Can't read '{0}' file in SCM({1}) in '{2}'. ***\n"
                                                        , poilabel(a), poilabel(no), fname));
                                                    //terminate(1);
                                                    throw new ApplicationException();
                                                }
                                            }
                                            //fclose(tscm.mapping.fp);
                                        }
                                    }
                                    break;

                                default:
                                    a = rword(tp);
                                    b = rword(tp);
                                    if (!chklabel(a) || !chklabel(b)) goto serror;
                                    tscm.mapping.color[0] = a;
                                    tscm.mapping.color[1] = b;
                                    a = rword(tp);
                                    b = rword(tp);
                                    if (!chkvalue(a) || !chkvalue(b)) goto serror;
                                    tscm.mapping.x = value[a - BASE_VALUE];
                                    tscm.mapping.y = value[b - BASE_VALUE];
                                    a = rword(tp);
                                    b = rword(tp);
                                    if (!chkvalue(a) || !chkvalue(b)) goto serror;
                                    if ((tscm.mapping.dx = value[a - BASE_VALUE]) == 0
                                    || (tscm.mapping.dy = value[b - BASE_VALUE]) == 0)
                                        goto verror;
                                    break;
                            }
                            break;

                        case P_SCALE:
                            a = rword(tp);
                            b = rword(tp);
                            c = rword(tp);
                            if (!chkvalue(a) || !chkvalue(b) || !chkvalue(c))
                                goto serror;
                            if ((tscm.mapping.sclx = value[a - BASE_VALUE]) <= 0
                                || (tscm.mapping.scly = value[b - BASE_VALUE]) <= 0
                                || (tscm.mapping.sclz = value[c - BASE_VALUE]) <= 0)
                                goto verror;
                            tscm.mapping.sclx = 1.0 / tscm.mapping.sclx;
                            tscm.mapping.scly = 1.0 / tscm.mapping.scly;
                            tscm.mapping.sclz = 1.0 / tscm.mapping.sclz;
                            break;

                        case P_RBPH:
                            a = rword(tp);
                            b = rword(tp);
                            c = rword(tp);
                            if (!chkvalue(a) || !chkvalue(b) || !chkvalue(c))
                                goto serror;
                            tscm.mapping.rb = value[a - BASE_VALUE] * RADIAN;
                            tscm.mapping.rp = value[b - BASE_VALUE] * RADIAN;
                            tscm.mapping.rh = value[c - BASE_VALUE] * RADIAN;
                            break;

                        case P_OXYZ:
                            a = rword(tp);
                            b = rword(tp);
                            c = rword(tp);
                            if (!chkvalue(a) || !chkvalue(b) || !chkvalue(c))
                                goto serror;
                            tscm.mapping.ox = value[a - BASE_VALUE];
                            tscm.mapping.oy = value[b - BASE_VALUE];
                            tscm.mapping.oz = value[c - BASE_VALUE];
                            break;

                        default:
                            goto serror;
                    }
                }
                /* value range check */
                if (!chk01(tscm.r) || !chk01(tscm.g)
                    || !chk01(tscm.b) || !chk01(tscm.fd)
                    || !chk01(tscm.fs1) || !chk01(tscm.fs2))
                    goto verror;
                if ((tscm.hm < 0 || tscm.hm > 4) || tscm.ka <= 0 || tscm.n <= 0
                    || (tscm.type < 0 || tscm.type > 2))
                    goto verror;
                //#if FALSE
                //	if (tscm.type == 2 && tscm.m_type != 0) {
                //	    fprintf(stderr, "*** Transparent primitive isn't allowed mapping. ***\n"); 
                //	    terminate(1);
                //	}
                //#endif
                if (!flag)
                {       /* INIT */
                    scmcpy(iscm, tscm);
                    continue;
                }

                if (tscm.source_no != 0)
                {/* compute inside parameters */
                    param[0] = tscm.mapping.rb;
                    param[1] = tscm.mapping.rp;
                    param[2] = tscm.mapping.rh;
                    matbph(tscm.mapping.gmat, param, dmyparam);
                    tscm.mapping.gmat[9] = 1.0;
                    invmat(tscm.mapping.gmat, tscm.mapping.gmat);
                }

                /* compute ka,kb,kc */
                t0 = 1.0 / tscm.ka;
                if (tscm.r <= 0) tscm.ka = -INFINITY;
                else tscm.ka = t0 * Math.Log(tscm.r);
                if (tscm.g <= 0) tscm.kb = -INFINITY;
                else tscm.kb = t0 * Math.Log(tscm.g);
                if (tscm.b <= 0) tscm.kc = -INFINITY;
                else tscm.kc = t0 * Math.Log(tscm.b);
                if (use_scm >= max_scm)
                {
                    more_scm();
                }
                uint iii = use_scm; uint jjj = max_scm; Debug.Log(iii); Debug.Log(jjj);
                scmcpy(scm[use_scm], tscm);
                if (tscm.hm == 0)
                {   /* Phong Model */
                    //if ((scm[use_scm].fac = (double(*)[3])malloc(30 * sizeof(double))) == NULL) {
                    //    fprintf(stderr, "*** No memory for allocation. ***\n");
                    //    terminate(1);
                    //}
                    scm[use_scm].fac = new double[10, 3];
                    for (i = 0; i < 21; i++)
                    {
                        w = i * 0.05;
                        work[i] = Math.Pow(w, tscm.hc);
                    }
                    maketable(scm[use_scm].fac, work);
                }
                else
                {           /* Blinn Model */
                    //if ((scm[use_scm].f = (double(*)[10][3])malloc(90 * sizeof(double))) == NULL) {
                    //    fprintf(stderr, "*** No memory for allocation. ***\n");
                    //    terminate(1);
                    //}
                    scm[use_scm].f = new double[3][,] { new double[10, 3], new double[10, 3], new double[10, 3] };
                    //if ((scm[use_scm].d = (double(*)[3])malloc(30 * sizeof(double))) == NULL) {
                    //    fprintf(stderr, "*** No memory for allocation. ***\n");
                    //    terminate(1);
                    //}
                    scm[use_scm].d = new double[10, 3];
                    for (i = 0; i < 21; i++)
                    {
                        w = i * 0.05;
                        if (i == 0) w = 1 / INFINITY;
                        switch (tscm.hm)
                        {
                            case 1:
                                if (tscm.hc == 0)
                                {
                                    work[i] = 0;
                                    break;
                                }
                                w = Math.Acos(w) / tscm.hc;
                                work[i] = Math.Exp(-w * w);
                                break;

                            case 2:
                                work[i] = Math.Pow(w, tscm.hc);
                                break;

                            case 3:
                                if (tscm.hc == 0)
                                {
                                    work[i] = 0;
                                    break;
                                }
                                w *= w;
                                work[i] = Math.Exp(-(1 - w) / (w * tscm.hc * tscm.hc)) / (w * w * tscm.hc * tscm.hc);
                                break;
                        }
                    }
                    maketable(scm[use_scm].d, work);
                    for (i = 0; i < 3; i++)
                    {
                        if (i == 0) w = tscm.r;
                        else if (i == 1) w = tscm.g;
                        else w = tscm.b;
                        if ((w = Math.Sqrt(w)) == 1) w = INFINITY;
                        else w = (1 + w) / (1 - w);
                        makerf(scm[use_scm].f[i], w);
                    }
                }
                if (tscm.type == 2)
                {   /* transparent primitive */
                    //if ((scm[use_scm].rf = (double(*)[10][3])malloc(60 * sizeof(double))) == NULL) {
                    //    fprintf(stderr, "*** No memory for allocation. ***\n");
                    //    terminate(1);
                    //}
                    scm[use_scm].rf = new double[2][,] {
                        new double[10,3], new double[10,3]
                    };
                    makerf(scm[use_scm].rf[0], tscm.n);
                    makerf(scm[use_scm].rf[1], 1.0 / tscm.n);
                }
                use_scm++;
            }

            temp_unlink(ref tp);        /* Unlink the temporary file */

            if (use_scm == 0)
            {
                Debug.LogError(String.Format("*** There is no SCM in '{0}'. ***\n", fname));
                //terminate(1);
                throw new ApplicationException();
            }

            for (i = 0; i < use_scm; i++)
                if (scm[i].source_no >= 3)
                {
                    for (j = 0; j < use_scm; j++)
                        if (scm[j].label == scm[i].mapping.color[0])
                        {
                            scm[i].mapping.scm[0] = j;
                            break;
                        }
                    if (j >= use_scm)
                    {
                        Debug.LogError(String.Format("*** Can't find SCM({0}) in '{1}'. ***\n"
                            , poilabel(scm[i].mapping.color[0]), fname));
                        //terminate(1);
                        throw new ApplicationException();
                    }
                    for (j = 0; j < use_scm; j++)
                        if (scm[j].label == scm[i].mapping.color[1])
                        {
                            scm[i].mapping.scm[1] = j;
                            break;
                        }
                    if (j >= use_scm)
                    {
                        Debug.LogError(String.Format("*** Can't find SCM({0}) in '{1}'. ***\n"
                            , poilabel(scm[i].mapping.color[1]), fname));
                        //terminate(1);
                        throw new ApplicationException();
                    }
                }

            return;

        /* Error happend!!! */
        serror:
            Debug.LogErrorFormat("*** Syntax error SCM({0}) in '{1}'. ***\n"
                , poilabel(no), fname);
            //terminate(1);
            throw new ApplicationException();

        verror:
            Debug.LogErrorFormat("*** Illegal value error SCM({0}) in '{1}'. ***\n"
                , poilabel(no), fname);
            //terminate(1);
            throw new ApplicationException();
        }

        public static void scmcpy(tag_scm a, tag_scm b)
        {
            int i;

            a.label = b.label;
            a.type = b.type;
            a.r = b.r;
            a.g = b.g;
            a.b = b.b;
            a.fd = b.fd;
            a.fh = b.fh;
            a.hm = b.hm;
            a.hc = b.hc;
            a.fs1 = b.fs1;
            a.fs2 = b.fs2;
            a.ka = b.ka;
            a.kb = b.kb;
            a.kc = b.kc;
            a.n = b.n;
            a.source_no = b.source_no;
            a.mapping.fp = b.mapping.fp;
            for (i = 0; i < 2; i++)
            {
                a.mapping.color[i] = b.mapping.color[i];
                a.mapping.scm[i] = b.mapping.scm[i];
            }
            a.mapping.x = b.mapping.x;
            a.mapping.y = b.mapping.y;
            a.mapping.dx = b.mapping.dx;
            a.mapping.dy = b.mapping.dy;
            a.mapping.mapping_no = b.mapping.mapping_no;
            for (i = 0; i < 10; i++)
                a.mapping.gmat[i] = b.mapping.gmat[i];
            a.mapping.ox = b.mapping.ox;
            a.mapping.oy = b.mapping.oy;
            a.mapping.oz = b.mapping.oz;
            a.mapping.rb = b.mapping.rb;
            a.mapping.rp = b.mapping.rp;
            a.mapping.rh = b.mapping.rh;
            a.mapping.sclx = b.mapping.sclx;
            a.mapping.scly = b.mapping.scly;
            a.mapping.sclz = b.mapping.sclz;
            a.mapping.mem = b.mapping.mem;
            a.mapping.fx = b.mapping.fx;
            a.mapping.fy = b.mapping.fy;
            a.fac = b.fac;
            a.f = b.f;
            a.d = b.d;
            a.rf = b.rf;
        }

        public static void maketable(double[,] a, double[] b)
        {
            int i;
            double x1, x2, x3;
            double y1, y2, y3;
            double delta;
            int j;

            for (i = 0; i < 10; i++)
            {
                x1 = i * 0.1;
                x2 = x1 + 0.05;
                x3 = (i + 1) * 0.1;
                y1 = b[i * 2];
                y2 = b[i * 2 + 1];
                y3 = b[i * 2 + 2];
                delta = 1.0 / ((x1 - x2) * (x2 - x3) * (x1 - x3));
                a[i, 0] = delta * ((x1 - x2) * (y3 - y1) - (x3 - x1) * (y1 - y2));
                a[i, 1] = delta * ((x3 + x1) * (x3 - x1) * (y1 - y2)
                           - (x1 + x2) * (x1 - x2) * (y3 - y1));
                a[i, 2] = delta * (x1 * x2 * y3 * (x1 - x2)
                           + y1 * x2 * x3 * (x2 - x3)
                           + x1 * y2 * x3 * (x3 - x1));
                for (j = 0; j < 3; j++)
                    if (Math.Abs(a[i, j]) > INFINITY)
                        a[i, j] = SGN(a[i, j]) * INFINITY;
            }
        }

        public static void makerf(double[,] a, double n)
        {
            int i;
            double y, c, w;
            double[] rf = new double[21];

            n *= n;
            for (i = 0; i < 21; i++)
            {
                c = i * 0.05;
                if ((y = n + c * c - 1) < 0)
                {
                    rf[i] = 1;
                    continue;
                }
                y = Math.Sqrt(y);
                if (y + c == 0)
                {
                    rf[i] = 0;
                    continue;
                }
                w = (y - c) / (y + c);
                rf[i] = w * w;
                w = (y - n * c) / (y + n * c);
                rf[i] = (rf[i] + w * w) * 0.5;
            }
            maketable(a, rf);
        }

        /* Calculate value */
        public static double cal_value(double v, double[,] tbl)
        {
            int i;

            i = (int)Math.Floor(v * 10);
            if (i < 0) i = 0;
            else if (i > 9) i = 9;
            return ((tbl[i, 0] * v + tbl[i, 1]) * v + tbl[i, 2]);
        }

        public static void pdcpy(tag_pd a, tag_pd b)
        {
            a.label = b.label;
            a.kind = b.kind;
            a.pa = b.pa;
            a.pb = b.pb;
            a.pc = b.pc;
            a.rb = b.rb;
            a.rp = b.rp;
            a.rh = b.rh;
            a.ox = b.ox;
            a.oy = b.oy;
            a.oz = b.oz;
            a.sg = b.sg;
            a.scm = b.scm;
            a.light = b.light;
            a.type = b.type;
            a.scl[0] = b.scl[0];
            a.scl[1] = b.scl[1];
            a.scl[2] = b.scl[2];
        }

        private const int ADD_PD = 30;
        public static void more_pd()
        {
            //uint no;
            //uint i;
            //tag_pd[] sub_pd;

            /* Extending of pd[] */
            Debug.Log("Extending of pd[].\n");
            //no = max_pd + ADD_PD;
            //if ((sub_pd = (struct tag_pd *)malloc(no* sizeof(struct tag_pd))) == NULL) {
            //fprintf(stderr, "*** No memory for allocation. ***\n");
            //terminate(1);
            //};
            //sub_pd = new tag_pd[no];
            //for (i = 0; i < use_pd; i++)
            //    pdcpy(sub_pd[i], pd[i]);
            //free((char*)pd);
            //pd = sub_pd;
            uint i = max_pd;
            max_pd += ADD_PD;
            Array.Resize(ref pd, (int)max_pd);
            for (; i < max_pd; i++) pd[i] = new tag_pd();
        }

        /* Convert pd data */
        public static void cvtpd(string fname)
        {
            bool flag;
            uint no = 0;
            uint i;
            uint a, b, c;
            tag_TEMP tp;
            //static struct tag_pd iipd = {
            //0, 3, 1.0, 1.0, 1.0, 0, 0, 0, 0, 0, 0, 1.0, 0, 0.1, 0, 1.0, 1.0, 1.0
            //};
            tag_pd iipd = new tag_pd();
            iipd.label = 0;
            iipd.kind = 3;
            iipd.pa = iipd.pb = iipd.pc = 1.0;
            iipd.rb = iipd.rp = iipd.rh = 0;
            iipd.ox = iipd.oy = iipd.oz = 0;
            iipd.sg = 1.0;
            iipd.scm = 0;
            iipd.light = 0.1;
            iipd.type = 0;
            iipd.scl[0] = iipd.scl[1] = iipd.scl[2] = 1.0;
            tag_pd tpd = new tag_pd();
            tag_pd ipd = new tag_pd();

            use_pd = 0;
            iipd.scm = max_scm;
            pdcpy(ipd, iipd);

            tp = tempfile(fname);   /* make temporary file */

            while (!chkeof(i = rword(tp)))
            {
                pdcpy(tpd, ipd);
                flag = false;
                if (i == P_PRIMITIVE) flag = true;
                else if (i != P_INIT) goto serror;
                no = P_INIT;
                if (flag)
                {
                    no = rword(tp);
                    if (!chklabel(no) || chkeof(no)) goto serror;
                    /* check multi definition of PD */
                    for (i = 0; i < use_pd; i++)
                        if (pd[i].label == no)
                        {
                            Debug.LogErrorFormat("*** Multi-definr PD({0}) in '{1}'. ***\n"
                                , poilabel(no), fname);
                            //terminate(1);
                            throw new ApplicationException();
                        }
                    tpd.label = no;
                }
                a = rword(tp);
                if (a != BASE_SEPA + '{') goto serror;
                while ((a = rword(tp)) != BASE_SEPA + '}')
                {
                    switch (a)
                    {
                        case P_KIND:
                            a = rword(tp);
                            if (!chkvalue(a)) goto serror;
                            tpd.kind = (int)value[a - BASE_VALUE];
                            break;

                        case P_PABC:
                            a = rword(tp);
                            b = rword(tp);
                            c = rword(tp);
                            if (!chkvalue(a) || !chkvalue(b) || !chkvalue(c))
                                goto serror;
                            tpd.pa = value[a - BASE_VALUE];
                            tpd.pb = value[b - BASE_VALUE];
                            tpd.pc = value[c - BASE_VALUE];
                            break;

                        case P_RBPH:
                            a = rword(tp);
                            b = rword(tp);
                            c = rword(tp);
                            if (!chkvalue(a) || !chkvalue(b) || !chkvalue(c))
                                goto serror;
                            tpd.rb = value[a - BASE_VALUE];
                            tpd.rp = value[b - BASE_VALUE];
                            tpd.rh = value[c - BASE_VALUE];
                            break;

                        case P_OXYZ:
                            a = rword(tp);
                            b = rword(tp);
                            c = rword(tp);
                            if (!chkvalue(a) || !chkvalue(b) || !chkvalue(c))
                                goto serror;
                            tpd.ox = value[a - BASE_VALUE];
                            tpd.oy = value[b - BASE_VALUE];
                            tpd.oz = value[c - BASE_VALUE];
                            break;

                        case P_SG:
                            a = rword(tp);
                            if (!chkvalue(a)) goto serror;
                            tpd.sg = (double)value[a - BASE_VALUE];
                            /* Warning : This is double precission value. */
                            break;

                        case P_SCM:
                            a = rword(tp);
                            if (!chklabel(a) || chkeof(a)) goto serror;
                            for (i = 0; i < use_scm; i++)
                                if (scm[i].label == a) break;
                            if (i >= use_scm)
                            {
                                Debug.LogErrorFormat("*** Illegal label of SCM({0})  in PD({1}) in '{2}'. ***\n"
                                    , poilabel(a), poilabel(no), fname);
                                //terminate(1);
                                throw new ApplicationException();
                            }
                            tpd.scm = i;
                            break;

                        case P_LIGHT:
                            a = rword(tp);
                            if (!chkvalue(a)) goto serror;
                            tpd.light = value[a - BASE_VALUE];
                            break;

                        case P_TYPE:
                            a = rword(tp);
                            if (!chkvalue(a)) goto serror;
                            tpd.type = (int)value[a - BASE_VALUE];
                            break;

                        case P_SCALE:
                            a = rword(tp);
                            if (!chkvalue(a)) goto serror;
                            tpd.scl[0] = tpd.scl[1] = tpd.scl[2] = value[a - BASE_VALUE];
                            a = rword(tp);
                            if (!chkvalue(a))
                            {
                                temp_ungetc(tp);
                                break;
                            }
                            tpd.scl[1] = value[a - BASE_VALUE];
                            a = rword(tp);
                            if (!chkvalue(a))
                                goto serror;
                            tpd.scl[2] = value[a - BASE_VALUE];
                            break;

                        default:
                            goto serror;
                    }
                }

                /* value range check */
                if (tpd.kind < 1 || tpd.kind > 5) goto verror;
                switch (tpd.kind)
                {
                    case 1:
                        if (tpd.pa <= 0 || tpd.pb <= 0 || tpd.pc <= 0)
                            goto verror;
                        break;

                    case 2:
                        if (tpd.pa == 0 && tpd.pb == 0 && tpd.pc == 0)
                            goto verror;
                        break;

                    case 3:
                        if ((tpd.pa <= 0 && tpd.pb <= 0 && tpd.pc <= 0)
                        || (tpd.pa == 0 || tpd.pb == 0 || tpd.pc == 0))
                            goto verror;
                        break;

                    case 4:
                        if ((tpd.pa <= 0 && tpd.pb <= 0 && tpd.pc <= 0)
                        || (tpd.pa * tpd.pb * tpd.pc >= 0))
                            goto verror;
                        break;

                    case 5:
                        if (tpd.pa <= 0 || tpd.pb <= 0 || tpd.pc <= 0
                        || Math.Abs(tpd.sg) <= 1.0)
                            goto verror;
                        break;
                }
                if (tpd.sg == 0 || tpd.light < 0
                    || (tpd.type < 0 || tpd.type > 2)
                    || tpd.scl[0] <= 0 || tpd.scl[1] <= 0 || tpd.scl[2] <= 0)
                    goto verror;
                if (flag && tpd.scm >= use_scm)
                {
                    Debug.LogErrorFormat("*** Don't appoint SCM in PD({0}) in '{1}'. ***\n"
                        , poilabel(no), fname);
                    //terminate(1);
                    throw new ApplicationException();
                }
                if (!flag)
                {
                    pdcpy(ipd, tpd);
                    continue;
                }
                if (use_pd >= max_pd)
                {
                    more_pd();
                }
//#if FALSE
//	if (tpd.kind == 2 && scm[tpd.scm].type == 2) {
//	    fprintf(stderr, "*** Transparent primitive isn't allowed plane. ***\n"); 
//	    terminate(1);
//	}
//#endif
                pdcpy(pd[use_pd++], tpd);
            }

            temp_unlink(ref tp);        /* Unlink the temporary file */

            if (use_pd == 0)
            {
                Debug.LogErrorFormat("*** There is no PD in '{0}'. ***\n", fname);
                //terminate(1);
                throw new ApplicationException();
            }
            return;

        /* Error happend !!! */
        serror:
            Debug.LogErrorFormat("*** Syntax error PD({0}) in '{1}'. ***\n"
                , poilabel(no), fname);
            //terminate(1);
            throw new ApplicationException();

        verror:
            Debug.LogErrorFormat("*** Illegal value error PD({0}) in '{1}'. ***\n"
                , poilabel(no), fname);
            //terminate(1);
            throw new ApplicationException();
        }

    }
}
