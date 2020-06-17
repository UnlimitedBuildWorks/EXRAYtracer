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
using static EXRAY.ray1;

namespace EXRAY
{
    public class ray2
    {
        /* definition of some use variables(local) */
        private static ulong count = 0;
        private static bool viewflag = false;

        /* definition of local struct */
        public class tag_list
        {
            public uint no;
            public tag_TEMP_SUB temp = new tag_TEMP_SUB();
            public tag_list forward;
        }
        private static tag_list start = new tag_list();

        /* Convert md data */
        public static void cvtmd(string fname)
        {
            tag_TEMP tp;
            int i;
            uint j;
            double[] mat = new double[10];
            double[] xyz = new double[3];

            tp = tempfile(fname);
            tp = prefix(tp, fname);

            start.forward = start;
            use_prim = use_atable = use_light = use_trans = max_meta_ball = 0;
            for (i = 0; i < 9; i++) mat[i] = 0;
            mat[0] = mat[4] = mat[8] = 1;
            for (i = 0; i < 3; i++) xyz[i] = 0;
            count = 0;
            viewflag = false;       /* There is a view point,or not. */

            /* Goto "WORLD" Module */
            cvtmdsub(tp, (uint)P_WORLD, mat, xyz, fname);

            if (!viewflag)
            {
                Debug.LogErrorFormat("*** There is no view-point in '{0}'. ***\n", fname);
                terminate(1);
            }
            if (use_prim == 0)
            {
                Debug.LogErrorFormat("*** There is no primitive in '{0}'. ***\n", fname);
                terminate(1);
            }
            if (use_atable == 0)
            {
                Debug.LogErrorFormat("*** There is no atable in '{0}'. ***\n", fname);
                terminate(1);
            }
            if (use_light == 0)
            {
                Debug.LogErrorFormat("*** There is no light in '{0}'. ***\n", fname);
                terminate(1);
            }
            //stack1 = (double(*)[2])malloc((20 * max_meta_ball + 10) * 2 * sizeof(double));
            stack1 = new double[20 * max_meta_ball + 10, 2];
            //stack2 = (double(*)[2])malloc((2 * max_meta_ball + 10) * 2 * sizeof(double));
            stack2 = new double[2 * max_meta_ball + 10, 2];
            //stack3 = (double(*)[2])malloc((20 * max_meta_ball + 10) * 2 * sizeof(double));
            stack3 = new double[20 * max_meta_ball + 10, 2];
            //meta_list = (POINTER*)malloc((max_meta_ball + 1) * sizeof(POINTER));
            meta_list = new uint[max_meta_ball + 1];
            //trans = (POINTER*)malloc((use_atable + 1) * sizeof(POINTER));
            trans = new uint[use_atable + 1];
            //if (stack1 == NULL || stack2 == NULL || stack3 == NULL
            //|| meta_list == NULL || trans == NULL)
            //{
            //    fprintf(stderr, "*** No memory for allocation. ***\n");
            //    terminate(1);
            //}

            /* make trans[] */
            for (j = 0; j < use_atable; j++)
            {
                if (scm[PRIM(atable[j, 0]).scm].type == 2)
                {
                    trans[use_trans++] = j;
                }
            }

            temp_unlink(ref tp);        /* Unlink the temporary file */
        }

        public static uint new_label(uint a, uint b)
        {
            uint i, no;
            //char ct[3 * LEN_LABEL];
            string ct;

            if (!chklabel(a)) return (b);
            //sprintf(ct, "%s%s", poilabel(a), poilabel(b));
            ct = poilabel(a) + poilabel(b);
            //ct[LEN_LABEL] = '\0';
            no = BASE_LABEL;
            for (i = 0; i < use_slabel; i++)
            {
                if (ct == slabel[i])
                {
                    no = i;
                    break;
                }
            }
            if (no >= BASE_LABEL)
            {
                if (use_label >= max_label)
                {
                    more_label();
                }
                no = use_label;
                for (i = 0; i < use_label; i++)
                    if (ct == label[i])
                    {
                        no = i;
                        break;
                    }
                if (no == use_label)
                {
                    label[use_label++] = ct;
                }
                no += BASE_LABEL;
            }
            return (no);
        }

        /* Processes of 'prefix' and 'unprefix' */
        public static tag_TEMP prefix(tag_TEMP tp, string fname)
        {
            tag_TEMP tps;
            uint pf = BASE_SEPA;
            uint a, b;
            uint m_no = P_EOF;
            int level;

            tps = temp_open();
            while (!chkeof(a = rword(tp)))
            {
                switch (a)
                {
                    default:
                        goto serror;

                    case P_PREFIX:
                        pf = rword(tp);
                        if (!chklabel(pf) || chkeof(pf)) goto serror;
                        break;

                    case P_UNPREFIX:
                        pf = BASE_SEPA;
                        break;

                    case P_MODULE:
                        wword(tps, a);
                        a = rword(tp);
                        if (chkeof(a) || !chklabel(a)) goto serror;
                        wword(tps, m_no = new_label(pf, a));
                        if (rword(tp) != BASE_SEPA + '{') goto serror;
                        wword(tps, (uint)(BASE_SEPA + '{'));
                        a = rword(tp);
                        while (a != BASE_SEPA + '}')
                        {
                            if (a == BASE_SEPA + '{')
                            {
                                level = 1;
                                wword(tps, a);
                                while (level > 0)
                                {
                                    if (chkeof(a = rword(tp))) goto serror;
                                    wword(tps, a);
                                    if (a == BASE_SEPA + '{') level++;
                                    else if (a == BASE_SEPA + '}') level--;
                                }
                                a = rword(tp);
                            }
                            else
                            {
                                if (chkeof(a) || !chklabel(a)) goto serror;
                                b = rword(tp);
                                if (b != BASE_SEPA + '(')
                                {
                                    wword(tps, a);
                                    a = b;
                                }
                                else
                                {
                                    wword(tps, new_label(pf, a));
                                    wword(tps, b);
                                    while ((a = rword(tp)) != BASE_SEPA + ')')
                                    {
                                        if (chkeof(a)) goto serror;
                                        wword(tps, a);
                                    }
                                    wword(tps, a);
                                    a = rword(tp);
                                }
                            }
                        }
                        wword(tps, a);
                        break;
                }
                m_no = P_EOF;
            }
            temp_unlink(ref tp);
            return (tps);

        serror:
            Debug.LogErrorFormat("*** Syntax error in MODULE(%s) in '{0}' file. ***\n"
                , poilabel(m_no), fname);
            terminate(1);
            throw new ApplicationException();
        }

        /* Make Bank-Pitch-Heading matrix program */
        public static void matbph(double[] mat, double[] p, double[] scl)
        {
            double sb, sp, sh, cb, cp, ch;

            sb = Math.Sin(p[0]);
            sp = Math.Sin(p[1]);
            sh = Math.Sin(p[2]);
            cb = Math.Cos(p[0]);
            cp = Math.Cos(p[1]);
            ch = Math.Cos(p[2]);
            mat[0] = scl[0] * (cb * ch - sb * sp * sh);
            mat[1] = scl[1] * (sb * ch + cb * sp * sh);
            mat[2] = scl[2] * (-cp * sh);
            mat[3] = scl[0] * (-sb * cp);
            mat[4] = scl[1] * (cb * cp);
            mat[5] = scl[2] * (sp);
            mat[6] = scl[0] * (cb * sh + sb * sp * ch);
            mat[7] = scl[1] * (sb * sh - cb * sp * ch);
            mat[8] = scl[2] * (cp * ch);
        }

        /* Multiply matrix program */
        public static void mulmat(double[] a, double[] b, double[] c) /* a[] = b[] * c[] */
        {
            int i;
            int j;
            double[] s = new double[9];

            for (j = 0; j < 8; j += 3)
            {
                for (i = 0; i < 3; i++)
                {
                    s[i + j] = b[i] * c[j]
                             + b[i + 3] * c[j + 1]
                             + b[i + 6] * c[j + 2];
                }
            }

            for (i = 0; i < 9; i++) a[i] = s[i];
        }

        public static void primcpy(tag_prim a, tag_prim b)
        {
            int i;

            /* Copy prim b to a */
            a.kind = b.kind;
            a.cx = b.cx;
            a.cy = b.cy;
            a.cz = b.cz;
            a.sg = b.sg;
            a.scm = b.scm;
            for (i = 0; i < 20; i++)
            {
                a.par[i] = b.par[i];
            }
            for (i = 0; i < 10; i++)
            {
                a.mat[i] = b.mat[i];
            }
            a.count = b.count;
            a.pdno = b.pdno;

        }

        private const int ADD_ATABLE = 100;
        public static void more_atable()
        {
            uint no;
            uint i;
            uint[,] sub_atable;

            /* Extending of atable[][] */
            Debug.Log("Extending of atable[][].\n");
            no = max_atable + ADD_ATABLE;
            //if ((sub_atable = (POINTER(*)[2])malloc(no * 2 * sizeof(POINTER))) == NULL) {
            //    fprintf(stderr, "*** No memory for allocation. ***\n");
            //    terminate(1);
            //};
            sub_atable = new uint[no, 2];
            for (i = 0; i < use_atable; i++)
            {
                sub_atable[i, 0] = atable[i, 0];
                sub_atable[i, 1] = atable[i, 1];
            }
            //free((char*)atable);
            atable = sub_atable;
            max_atable += ADD_ATABLE;
        }

        /* Subroutine of cvtmd() */
        public static void cvtmdsub(tag_TEMP tp, uint m_no, double[] mat, double[] xyz, string fname)
        {
            int klevel;
            int i;
            uint j;
            uint sig_scm;
            uint top, bottom;
            uint a, b;
            ulong scount;
            bool chk;
            bool list_flag;
            tag_TEMP_SUB sub_tp = new tag_TEMP_SUB();
            tag_list list;
            double[] smat = new double[10];
            double[] sxyz = new double[3];
            double[] par = new double[6];
            double[] scl = new double[3];

            scount = count++;

            list_flag = false;
            for (list = start.forward; list != start; list = list.forward)
            {
                if (list.no == m_no)
                {
                    list_flag = true;
                    break;
                }
            }
            if (list_flag)
            {
                temp_seek(tp, list.temp);
            }
            else
            {
                /* search the appointed MODULE */
                temp_rewind(tp);    /* locate the file-pointer to File's top */

                while (!chkeof(a = rword(tp)))
                {
                    if (a != P_MODULE) goto serror;
                    if ((a = rword(tp)) == m_no)
                    {   /* found */
                        break;
                    }
                    if (!chklabel(a)) goto serror;
                    klevel = 0;
                    do
                    {
                        a = rword(tp);
                        if (chkeof(a)) goto serror;
                        if (a == BASE_SEPA + '{') klevel++;
                        else if (a == BASE_SEPA + '}') klevel--;
                    } while (klevel > 0);
                }
                if (chkeof(a))
                {   /* There is no appointed MODULE */
                    Debug.LogErrorFormat("*** There is no '{0}' module in '%{1}'. ***\n"
                        , poilabel(m_no), fname);
                    terminate(1);
                }
                if (rword(tp) != BASE_SEPA + '{') goto serror;
                //if ((list = (struct tag_list *)malloc(sizeof(struct tag_list))) == NULL) {
                //fprintf(stderr, "*** No memory for allocation. ***\n");
                //terminate(1);
                //}
                list = new tag_list();
                /* Add list */
                list.forward = start.forward;
                start.forward = list;
                list.no = m_no;
                temp_tell(tp, list.temp);
            }
            a = rword(tp);
            while (a != BASE_SEPA + '}')
            {
                if (chkeof(a)) goto serror;
                if (a == BASE_SEPA + '{')
                {

                    /* { <LABEL>,... } */

                    top = use_prim;
                    chk = false;
                    a = rword(tp);
                    while (a != BASE_SEPA + '}')
                    {
                        if (!chklabel(a)) goto serror;
                        sig_scm = max_scm;
                    LOOP_BACK:
                        if ((b = rword(tp)) != BASE_SEPA + '(')
                        {
                            /* No parameter */
                            for (i = 0; i < 9; i++) smat[i] = mat[i];
                            for (i = 0; i < 3; i++) sxyz[i] = xyz[i];
                        }
                        else
                        {
                            /* Parameters */
                            b = rword(tp);
                            if (!chkvalue(b))
                                if (chkeof(b) || !chklabel(b)) goto serror;
                                else
                                {
                                    for (j = 0; j < use_scm; j++)
                                    {
                                        if (scm[j].label == b) break;
                                    }
                                    if (j >= use_scm)
                                    {
                                        Debug.LogErrorFormat("*** Illegal label of SCM({0}) in MODULE(%s) in '{1}' file. ***\n"
                                            , poilabel(b), poilabel(m_no)

                                            , fname);
                                        terminate(1);
                                    }
                                    sig_scm = j;
                                    if (rword(tp) != BASE_SEPA + ')')
                                    {
                                        goto serror;
                                    }
                                    goto LOOP_BACK;
                                }
                            if ((scl[0] = scl[1] = scl[2] = value[b - BASE_VALUE]) <= 0)
                            {
                                goto serror;
                            }
                            for (i = 0; i < 6; i++)
                            {
                                b = rword(tp);
                                if (!chkvalue(b)) goto serror;
                                par[i] = value[b - BASE_VALUE];
                            }
                            b = rword(tp);
                            if (chkvalue(b))
                            {
                                if ((scl[1] = par[0]) <= 0 || (scl[2] = par[1]) <= 0)
                                {
                                    goto serror;
                                }
                                for (i = 2; i < 6; i++) par[i - 2] = par[i];
                                par[4] = value[b - BASE_VALUE];
                                b = rword(tp);
                                if (!chkvalue(b)) goto serror;
                                par[5] = value[b - BASE_VALUE];
                                b = rword(tp);
                            }
                            for (i = 0; i < 3; i++) par[i] *= RADIAN;
                            if (b != BASE_SEPA + ')') goto serror;
                            b = rword(tp);
                            matbph(smat, par, scl);
                            mulmat(smat, smat, mat);
                            for (i = 0; i < 3; i++)
                            {
                                sxyz[i] = xyz[i] + (par[3] * mat[i * 3]
                                            + par[4] * mat[i * 3 + 1]
                                            + par[5] * mat[i * 3 + 2]);
                            }
                        }
                        chk = chk || makeprim(a, smat, sxyz, scount, m_no

                                      , fname, sig_scm);
                        a = b;
                    }
                    bottom = use_prim - 1;
                    if (!chk && (top > bottom))
                    {
                        goto serror;    /* then {} */
                    }
                    if (use_atable >= max_atable)
                    {
                        more_atable();
                    }
                    if (!chk)
                    {
                        atable[use_atable, 0] = top;
                        atable[use_atable++, 1] = bottom;
                        if (maketrans(use_atable - 1))
                        {
                            Debug.LogErrorFormat("*** Opaque primitives and transparent ones are in the same and-table in MODULE({0}) in '{1}'. ***\n"
                                , poilabel(m_no), fname);
                            terminate(1);
                        }
                        if (top != bottom)
                        {
                            for (j = top; j <= bottom; j++)
                            {
                                if (PRIM(j).kind == 5)
                                {
                                    Debug.LogErrorFormat("*** Meta-Ball is in the and-table with another primitives in MODULE({0}) in '{1}'. ***\n"
                                        , poilabel(m_no), fname);
                                    terminate(1);
                                }
                            }
                        }
                    }
                    else if (bottom >= top && use_prim != 0)
                    {
                        Debug.LogErrorFormat("*** The light or view-point is in the and-table with normal primitives in '{0}' module in '{1}'. ***\n"
                            , poilabel(m_no), fname);
                        terminate(1);
                    }
                    a = rword(tp);
                    continue;
                }
                if (!chklabel(a)) goto serror;

                if ((b = rword(tp)) != BASE_SEPA + '(')
                {   /* <LABEL> */
                    top = bottom = use_prim;
                    chk = makeprim(a, mat, xyz, scount, m_no, fname, max_scm);
                    if (use_atable >= max_atable)
                    {
                        more_atable();
                    }
                    if (!chk)
                    {
                        atable[use_atable, 0] = top;
                        atable[use_atable++, 1] = bottom;
                        if (maketrans(use_atable - 1))
                        {
                            Debug.LogErrorFormat("*** Opaque primitives and transparent ones are in the same and-table in MODULE({0}) in '{1}'. ***\n"
                                , poilabel(m_no), fname);
                            terminate(1);
                        }
                    }
                    a = b;
                    continue;
                }

                /* <LABEL>([<VALUE>,...]) */

                if ((b = rword(tp)) == BASE_SEPA + ')')
                {
                    /* There is no parameters */
                    for (i = 0; i < 9; i++) smat[i] = mat[i];
                    for (i = 0; i < 3; i++) sxyz[i] = xyz[i];
                }
                else
                {           /* There are parameters */
                    if (!chkvalue(b)) goto serror;
                    scl[0] = scl[1] = scl[2] = value[b - BASE_VALUE];
                    if (scl[0] <= 0) goto serror;
                    b = rword(tp);
                    for (i = 0; i < 6; i++)
                    {
                        if (!chkvalue(b)) goto serror;
                        par[i] = value[b - BASE_VALUE];
                        b = rword(tp);
                    }
                    if (chkvalue(b))
                    {
                        if ((scl[1] = par[0]) <= 0 || (scl[2] = par[1]) <= 0)
                        {
                            goto serror;
                        }
                        for (i = 2; i < 6; i++) par[i - 2] = par[i];
                        par[4] = value[b - BASE_VALUE];
                        b = rword(tp);
                        if (!chkvalue(b)) goto serror;
                        par[5] = value[b - BASE_VALUE];
                        b = rword(tp);
                    }
                    for (i = 0; i < 3; i++) par[i] *= RADIAN;
                    if (b != BASE_SEPA + ')') goto serror;

                    matbph(smat, par, scl);
                    mulmat(smat, smat, mat);
                    for (i = 0; i < 3; i++)
                    {
                        sxyz[i] = xyz[i] + (par[3] * mat[i * 3]
                                    + par[4] * mat[i * 3 + 1]
                                    + par[5] * mat[i * 3 + 2]);
                    }
                }
                /* record the locating of file pointer */
                temp_tell(tp, sub_tp);
                /* call child MODULE */
                cvtmdsub(tp, a, smat, sxyz, fname);
                /* move locating of file pointer to sub_tp */
                temp_seek(tp, sub_tp);
                a = rword(tp);
            }               /* end of process of MODULE */
            return;

        serror:
            Debug.LogErrorFormat("*** Syntax error MODULE({0}) in '{1}'. ***\n"
                , poilabel(m_no), fname);
            terminate(1);
        }   /* end of cvtmdsub() */

        /* make trans[] program */
        public static bool maketrans(uint no)
        {
            uint i;
            uint top, bottom;
            int check;

            top = atable[no, 0];
            bottom = atable[no, 1];
            check = (scm[PRIM(top).scm].type == 2 ? TRUE : FALSE);
            for (i = top + 1; i <= bottom; i++)
            {
                if (check != (scm[PRIM(i).scm].type == 2 ? TRUE : FALSE))
                {
                    /* Error */
                    return (true);
                }
            }
            return (false);
        }

        /* compute inverse matrix program */
        public static void invmat(double[] b, double[] a)
        {
            int i;
            double[] c = new double[9];
            double det;

            c[0] = a[4] * a[8] - a[5] * a[7];
            c[3] = a[5] * a[6] - a[3] * a[8];
            c[6] = a[3] * a[7] - a[4] * a[6];

            c[1] = a[2] * a[7] - a[1] * a[8];
            c[4] = a[0] * a[8] - a[2] * a[6];
            c[7] = a[1] * a[6] - a[0] * a[7];

            c[2] = a[1] * a[5] - a[2] * a[4];
            c[5] = a[2] * a[3] - a[0] * a[5];
            c[8] = a[0] * a[4] - a[1] * a[3];

            /* Determination */
            det = 1.0 / (a[0] * c[0] + a[1] * c[3] + a[2] * c[6]);
            for (i = 0; i < 9; i++) b[i] = c[i] * det;
        }

        private const int ADD_PRIM = 100;
        public static void more_prim()
        {
            //uint no;
            //uint i;
            //tag_prim[] sub_prim;

            /*Extending of prim[] */
            Debug.Log("Extending of prim[].\n");
            //no = max_prim + ADD_PRIM;

            //if ((sub_prim = (struct tag_prim *)malloc(no* sizeof(struct tag_prim))) == NULL) {
            //fprintf(stderr, "*** No memory for allocation. ***\n");
            //terminate(1);
            //}
            //sub_prim = new tag_prim[no];
            //for (i = 0; i < use_prim; i++) primcpy(sub_prim[i], prim[i]);
            //free((char*) prim);
            //prim = sub_prim;
            uint i = max_prim;
            max_prim += ADD_PRIM;
            Array.Resize(ref prim, (int)max_prim);
            for (; i < max_prim; i++) prim[i] = new tag_prim();
        }

        public static void lightcpy(tag_light a, tag_light b)
        {
            a.cx = b.cx;
            a.cy = b.cy;
            a.cz = b.cz;
            a.r = b.r;
            a.g = b.g;
            a.b = b.b;
            a.light = b.light;
        }

        private const int ADD_LIGHT = 5;
        public static void more_light()
        {
            //uint no;
            //uint i;
            //tag_light[] sub_light;

            /*Extending of light[] */
            Debug.Log("Extending of light[].\n");
            //no = max_light + ADD_LIGHT;
            //if ((sub_light = (struct tag_light *)malloc(no* sizeof(struct tag_light))) == NULL) {
            //fprintf(stderr, "*** No memory for allocation. ***\n");
            //terminate(1);
            //}
            //sub_light = new tag_light[no];

            //for (i = 0; i < use_light; i++) lightcpy(sub_light[i], light[i]);
            //free((char*)light);
            //light = sub_light;
            uint i = max_light;
            max_light += ADD_LIGHT;
            Array.Resize(ref light, (int)max_light);
            for (; i < max_light; i++) light[i] = new tag_light();
        }

        /* make prim program */
        private const double GREATER_LIMIT = (1E+10);
        public static bool makeprim(uint p_no, double[] mat, double[] xyz, ulong scount
                 , uint m_no, string fname, uint sig_scm)
        {
            uint i;
            uint j;
            tag_pd tpd;
            tag_prim tprim;
            double pa, pb, pc;
            double d, w, a, b;
            double[] smat = new double[10], par = new double[3], sxyz = new double[3];
            double[] gmat = new double[10];
            double a1, a2, a3, a4;

            /* search appointed pd[] */
            for (i = 0; i < use_pd; i++)
            {
                if (pd[i].label == p_no) break;
            }
            if (i >= use_pd)
            {       /* There is no appointed pd[] */
                Debug.LogErrorFormat("*** There is no PRIMITIVE({0}) appointed by MODULE({1}) in '{2}'. ***\n"
                    , poilabel(p_no), poilabel(m_no), fname);
                terminate(1);
            }
            tpd = pd[i];       /* tpd is appointed MODULE */
            if (use_prim >= max_prim)
            {
                more_prim();
            }
            tprim = PRIM(use_prim);
            /* copy parameters pd[] to PRIM() */
            tprim.count = scount;
            tprim.pdno = i;
            tprim.kind = tpd.kind;
            tprim.sg = tpd.sg;
            if (sig_scm >= use_scm) tprim.scm = tpd.scm;
            else tprim.scm = sig_scm;
            par[0] = RADIAN * tpd.rb;
            par[1] = RADIAN * tpd.rp;
            par[2] = RADIAN * tpd.rh;
            matbph(smat, par, tpd.scl);
            mulmat(smat, smat, mat);
            for (i = 0; i < 3; i++)
            {
                sxyz[i] = xyz[i] + (tpd.ox * mat[i * 3]
                            + tpd.oy * mat[i * 3 + 1]
                            + tpd.oz * mat[i * 3 + 2]);
            }
            tprim.cx = sxyz[0];
            tprim.cy = sxyz[1];
            tprim.cz = sxyz[2];
            invmat(gmat, smat);
            for (i = 0; i < 9; i++) tprim.mat[i] = gmat[i];
            /* compute primitive's parameters */
            switch (tprim.kind)
            {
                case 1:         /* CHOKUHOUTAI */
                    pa = tpd.pa * smat[0] + tpd.pb * smat[1] + tpd.pc * smat[2];
                    pb = tpd.pa * smat[3] + tpd.pb * smat[4] + tpd.pc * smat[5];
                    pc = tpd.pa * smat[6] + tpd.pb * smat[7] + tpd.pc * smat[8];
                    for (i = 0; i < 3; i++)
                    {
                        a1 = smat[i];
                        a2 = smat[i + 3];
                        a3 = smat[i + 6];
                        a1 *= (a4 = 1.0 / Math.Sqrt(a1 * a1 + a2 * a2 + a3 * a3));
                        a2 *= a4;
                        a3 *= a4;
                        a4 = -(a1 * pa + a2 * pb + a3 * pc);
                        tprim.par[i * 4] = a1;
                        tprim.par[i * 4 + 1] = a2;
                        tprim.par[i * 4 + 2] = a3;
                        tprim.par[i * 4 + 3] = a4;
                    }
                    for (i = 12; i < 15; i++) tprim.par[i] = 0;
                    pa = tpd.pa;
                    pb = tpd.pb;
                    pc = tpd.pc;
                    /* compute Xm,Ym,Zm */
                    for (i = 0; i < 4; i++)
                    {
                        pa = -pa;
                        if (i == 2) pb = -pb;
                        for (j = 0; j < 3; j++)
                        {
                            a1 = Math.Abs(pa * smat[j * 3]
                                  + pb * smat[j * 3 + 1]
                                  + pc * smat[j * 3 + 2]);
                            if (a1 > tprim.par[12 + j])
                            {
                                tprim.par[12 + j] = a1;
                            }
                        }
                    }
                    break;

                case 2:         /* HEIMEN */
                    pa = tpd.pa;
                    pb = tpd.pb;
                    pc = tpd.pc;

                    for (i = 0; i < 3; i++)
                    {
                        tprim.par[i] = pa * gmat[i]
                        + pb * gmat[i + 3]
                            + pc * gmat[i + 6];
                    }

                    tprim.par[0] *= (d = 1.0 / Math.Sqrt(tprim.par[0] * tprim.par[0]
                                     + tprim.par[1] * tprim.par[1]
                                     + tprim.par[2] * tprim.par[2]));
                    tprim.par[1] *= d;
                    tprim.par[2] *= d;
                    break;

                case 3:         /* 2JI KYOKUMEN */
                case 4:         /* SUIKEI */
                case 5:         /* Meta-Ball */
                    pa = 1 / tpd.pa;
                    pb = 1 / tpd.pb;
                    pc = 1 / tpd.pc;
                    if (tprim.kind != 4) d = -1.0;
                    else d = 0;
                    if (Math.Abs(pa) < GREATER_LIMIT) pa = SGN(pa) * pa * pa;
                    else pa = SGN(pa) * INFINITY;
                    if (Math.Abs(pb) < GREATER_LIMIT) pb = SGN(pb) * pb * pb;
                    else pb = SGN(pb) * INFINITY;
                    if (Math.Abs(pc) < GREATER_LIMIT) pc = SGN(pc) * pc * pc;
                    else pc = SGN(pc) * INFINITY;
                    if ((pa < 0 || pb < 0 || pc < 0)
                        && SGN(pa) * SGN(pb) * SGN(pc) > 0)
                    {
                        pa = -pa;
                        pb = -pb;
                        pc = -pc;
                        d = -d;
                    }
                    /* compute parameter A-G */
                    for (i = 0; i < 3; i++)
                    {
                        tprim.par[i] = pa * gmat[i] * gmat[i]
                        + pb * gmat[i + 3] * gmat[i + 3]
                            + pc * gmat[i + 6] * gmat[i + 6];
                    }
                    tprim.par[3] = pa * gmat[1] * gmat[2]
                        + pb * gmat[4] * gmat[5]
                        + pc * gmat[7] * gmat[8];
                    tprim.par[4] = pa * gmat[0] * gmat[2]
                            + pb * gmat[3] * gmat[5]
                            + pc * gmat[6] * gmat[8];
                    tprim.par[5] = pa * gmat[0] * gmat[1]
                            + pb * gmat[3] * gmat[4]
                            + pc * gmat[6] * gmat[7];
                    tprim.par[6] = d;

                    if (tprim.kind != 5) break;
                    /* Meta-Ball only */
                    /* W(eight) is tprim.sg */
                    a = Math.Abs(w = tprim.sg);
                    /* computing of b */
                    if (a <= 1.5)
                    {   /* I */
                        b = Math.Sqrt(3.0 * a / (a - 1.0));
                    }
                    else
                    {       /* II */
                        b = 1.0 / (1.0 - Math.Sqrt(2.0 / (3.0 * a)));
                    }
                    tprim.par[6] = (-b * b);
                    tprim.par[7] = b;
                    tprim.par[8] = b / 3.0;
                    tprim.par[9] = 1.0 / b;
                    tprim.par[10] = 6.0 * w / (b * b);
                    tprim.par[11] = 3.0 * w / b;
                    tprim.par[12] = w;
                    tprim.par[13] = 1.5 * w;
                    tprim.par[14] = b * b;
                    break;
            }               /* end of switch */

            switch (tpd.type)
            {
                case 0:         /* Normal primitive */
                    use_prim++;
                    if (tprim.kind == 5) max_meta_ball++;
                    return (false);

                case 1:         /* Light */
                    if (use_light >= max_light)
                    {
                        more_light();
                    }
                    i = use_light++;
                    light[i].cx = tprim.cx;
                    light[i].cy = tprim.cy;
                    light[i].cz = tprim.cz;
                    j = tprim.scm;
                    light[i].r = scm[j].r;
                    light[i].g = scm[j].g;
                    light[i].b = scm[j].b;
                    light[i].light = tpd.light;
                    return (true);

                case 2:         /* View Point */
                    if (viewflag)
                    {   /* Double define view-point */
                        Debug.LogErrorFormat("*** There are two or more view-point in MODULE({0}) in '{1}'. ***\n"
                            , poilabel(m_no), fname);
                        terminate(1);
                    }
                    viewflag = true;
                    for (i = 0; i < 9; i++) view.mat[i] = smat[i];
                    view.cx = tprim.cx;
                    view.cy = tprim.cy;
                    view.cz = tprim.cz;
                    return (true);

                default:
                    Debug.LogErrorFormat("*** Inside error(makeprim()). ***\n");
                    terminate(1);
                    break;
            }               /* End of switch */
            return (false);
        }				/* End of makeprim */

        public static int chk2D(double a, double b, double c, double d, double e, double f
      , double x1, double y1, double x2, double y2, int sg)
        {
            double m1, m2, m3;
            double n1, n2, n3;
            int i;
            double xc, yc;

            /* process I-1 */
            m1 = n1 = c * c - a * b;
            m2 = c * d - a * e;
            m3 = d * d - a * f;
            n2 = c * e - b * d;
            n3 = e * e - b * f;
            if (a != 0)
            {
                if ((m1 < 0 && m3 - m2 * m2 / m1 < 0)
                    || (m1 == 0 && m2 == 0 && m3 < 0)) goto L_CHECK;
            }
            else if (b != 0)
            {
                if (n1 == 0 && n2 == 0 && n3 < 0) goto L_CHECK;
            }
            else if (c == 0 && d == 0 && e == 0) goto L_CHECK;

            /* process I-5 */
            if ((n1 * x1 + 2 * n2) * x1 + n3 >= 0)
            {
                if (((i = SGN(b * y1 + c * x1 + e)) != SGN(b * y2 + c * x1 + e))
                    && (i != sg))
                {
                    goto L_MIDDLE;
                }
            }

            /* process I-5 */
            if ((n1 * x2 + 2 * n2) * x2 + n3 >= 0)
            {
                if (((i = SGN(b * y1 + c * x2 + e)) != SGN(b * y2 + c * x2 + e))
                    && (i != sg))
                {
                    goto L_MIDDLE;
                }
            }

            /* process I-2 */
            if (m1 >= 0) goto L_CHECK;

            /* HEI KYOKUMEN */
            xc = -n2 / n1;
            yc = -m2 / m1;
            if (x1 <= xc && xc <= x2 && y1 <= yc && yc <= y2)
            {
                if (SGN((a * xc + 2 * (c * yc + d)) * xc + (b * yc + 2 * e) * yc + f) != sg)
                {
                    goto L_MIDDLE;
                }
            }

        L_CHECK:
            return (sg);

        L_MIDDLE:
            return ((int)MIDDLE);
        }

        public static int chk3D1(tag_prim tprim, double x1, double y1, double z1
       , double x2, double y2, double z2)
        {
            bool flag;
            double max, min;
            double a1, a2, a3, a4;
            double xm, ym, zm;

            xm = tprim.par[12];
            ym = tprim.par[13];
            zm = tprim.par[14];
            if (-xm < x2 && x1 <= xm
            && -ym < y2 && y1 <= ym
            && -zm <= z2 && z1 <= zm)
            {
                flag = true;
            }
            else
            {
                goto L_OUT;
            }

            /* process II-1 */
            a1 = tprim.par[0];
            a2 = tprim.par[1];
            a3 = tprim.par[2];
            a4 = tprim.par[3];
            if (a1 >= 0)
            {
                max = a1 * x2;
                min = a1 * x1;
            }
            else
            {
                max = a1 * x1;
                min = a1 * x2;
            }
            if (a2 >= 0)
            {
                max += a2 * y2;
                min += a2 * y1;
            }
            else
            {
                max += a2 * y1;
                min += a2 * y2;
            }
            if (a3 >= 0)
            {
                max += a3 * z2;
                min += a3 * z1;
            }
            else
            {
                max += a3 * z1;
                min += a3 * z2;
            }
            if (a4 > -min || a4 > max) goto L_OUT;
            if (a4 > -max || a4 > min) flag = false;

            /* process II-1 */
            a1 = tprim.par[4];
            a2 = tprim.par[5];
            a3 = tprim.par[6];
            a4 = tprim.par[7];
            if (a1 >= 0)
            {
                max = a1 * x2;
                min = a1 * x1;
            }
            else
            {
                max = a1 * x1;
                min = a1 * x2;
            }
            if (a2 >= 0)
            {
                max += a2 * y2;
                min += a2 * y1;
            }
            else
            {
                max += a2 * y1;
                min += a2 * y2;
            }
            if (a3 >= 0)
            {
                max += a3 * z2;
                min += a3 * z1;
            }
            else
            {
                max += a3 * z1;
                min += a3 * z2;
            }
            if (a4 > -min || a4 > max) goto L_OUT;
            if (a4 > -max || a4 > min) flag = false;

            /* process II-1 */
            a1 = tprim.par[8];
            a2 = tprim.par[9];
            a3 = tprim.par[10];
            a4 = tprim.par[11];
            if (a1 >= 0)
            {
                max = a1 * x2;
                min = a1 * x1;
            }
            else
            {
                max = a1 * x1;
                min = a1 * x2;
            }
            if (a2 >= 0)
            {
                max += a2 * y2;
                min += a2 * y1;
            }
            else
            {
                max += a2 * y1;
                min += a2 * y2;
            }
            if (a3 >= 0)
            {
                max += a3 * z2;
                min += a3 * z1;
            }
            else
            {
                max += a3 * z1;
                min += a3 * z2;
            }
            if (a4 > -min || a4 > max) goto L_OUT;
            if (a4 > -max || a4 > min) flag = false;
            if (!flag)
            {       /* flag is FALSE */
                return ((int)MIDDLE);
            }
            /* IN */
            if (tprim.sg > 0)
            {   /* inside is true */
                return ((int)FULL);
            }
            /* outside is true */
            return ((int)EMPTY);

        L_OUT:              /* OUT */
            if (tprim.sg > 0)
            {       /* inside is true */
                return ((int)EMPTY);
            }
            /* outside is true */
            return ((int)FULL);
        }

        public static int chk3D2(tag_prim tprim, double x1, double y1, double z1
       , double x2, double y2, double z2)
        {
            int i;
            int sg = 0;
            double x, y, z;
            double nx, ny, nz;

            x = x1;
            nx = tprim.par[0]; /* Normal vectors of plane */
            ny = tprim.par[1];
            nz = tprim.par[2];
            for (i = 0; i < 8; i++)
            {
                if (i == 4) x = x2;
                if (0 != (i & 2)) y = y1;
                else y = y2;
                if (0 != (i & 1)) z = z1;
                else z = z2;
                if (i == 0) sg = SGN(nx * x + ny * y + nz * z);
                else if (sg != SGN(nx * x + ny * y + nz * z))
                {
                    return ((int)MIDDLE);
                }
            }
            if (tprim.sg * sg > 0)
            {       /* inside is true */
                return ((int)EMPTY);
            }
            /* outside is true */
            return ((int)FULL);
        }

        public static int chk3D3(tag_prim tprim, double x1, double y1, double z1
       , double x2, double y2, double z2)
        {
            double x, y, z;
            double a, b, c;
            int i;
            int sg = 0;
            int sgs;
            double d, e, f, g;

            a = tprim.par[0];
            b = tprim.par[1];
            c = tprim.par[2];
            d = tprim.par[3];
            e = tprim.par[4];
            f = tprim.par[5];
            g = tprim.par[6];
            x = x1;
            for (i = 0; i < 8; i++)
            {
                if (i == 4) x = x2;
                if (0 != (i & 2)) y = y1;
                else y = y2;
                if (0 != (i & 1)) z = z1;
                else z = z2;
                if (i == 0)
                {
                    sg = SGN((a * x + 2 * (e * z + f * y)) * x
                         + (b * y + 2 * d * z) * y + c * z * z + g);
                    if (x1 <= 0 && x2 >= 0 && y1 <= 0 && y2 >= 0
                    && z1 <= 0 && z2 >= 0)
                    {
                        if (SGN(g) != sg) goto L_MIDDLE;
                    }
                }
                else if (sg != SGN((a * x + 2 * (e * z + f * y)) * x
                               + (b * y + 2 * d * z) * y + c * z * z + g))
                {
                    goto L_MIDDLE;
                }
            }

            /* check in x=x1 */
            sgs = chk2D(b, c, d, f * x1, e * x1, a * x1 * x1 + g, y1, z1, y2, z2, sg);
            if (sgs == MIDDLE) goto L_MIDDLE;

            /* check in x=x2 */
            sgs = chk2D(b, c, d, f * x2, e * x2, a * x2 * x2 + g, y1, z1, y2, z2, sg);
            if (sgs == MIDDLE) goto L_MIDDLE;

            /* check in y=y1 */
            sgs = chk2D(c, a, e, d * y1, f * y1, b * y1 * y1 + g, z1, x1, z2, x2, sg);
            if (sgs == MIDDLE) goto L_MIDDLE;

            /* check in y=y2 */
            sgs = chk2D(c, a, e, d * y2, f * y2, b * y2 * y2 + g, z1, x1, z2, x2, sg);
            if (sgs == MIDDLE) goto L_MIDDLE;

            /* check in z=z1 */
            sgs = chk2D(a, b, f, e * z1, d * z1, c * z1 * z1 + g, x1, y1, x2, y2, sg);
            if (sgs == MIDDLE) goto L_MIDDLE;

            /* check in z=z2 */
            sgs = chk2D(a, b, f, e * z2, d * z2, c * z2 * z2 + g, x1, y1, x2, y2, sg);
            if (sgs == MIDDLE) goto L_MIDDLE;

            /* CHECK */
            return ((int)(sg * SGN(tprim.sg)));

        L_MIDDLE:
            return ((int)MIDDLE);
        }

        public static int chksolid(uint no, double x1, double y1, double z1
         , double x2, double y2, double z2)
        {
            int rvalue;
            tag_prim tprim;
            double lx1, ly1, lz1;
            double lx2, ly2, lz2;
            double swap;

            tprim = PRIM(no);
            lx1 = x1 - tprim.cx;   /* convert global coordinates */
            ly1 = y1 - tprim.cy;   /*      to  local coordinates */
            lz1 = z1 - tprim.cz;
            lx2 = x2 - tprim.cx;
            ly2 = y2 - tprim.cy;
            lz2 = z2 - tprim.cz;
            switch (tprim.kind)
            {
                case 1:         /* CHOKUHOUTAI */
                    return (chk3D1(tprim, lx1, ly1, lz1, lx2, ly2, lz2));

                case 2:         /* HEIMEN */
                    return (chk3D2(tprim, lx1, ly1, lz1, lx2, ly2, lz2));

                case 3:         /* 2JI KYOKUMEN */
                case 4:
                    return (chk3D3(tprim, lx1, ly1, lz1, lx2, ly2, lz2));

                case 5:         /* Meta-Ball */
                    swap = tprim.sg;
                    tprim.sg = 1.0;
                    rvalue = chk3D3(tprim, lx1, ly1, lz1, lx2, ly2, lz2);
                    tprim.sg = swap;
                    return (rvalue);

                default:
                    Debug.LogError("*** Inside error(chksolid()). ***\n");
                    terminate(1);
                    break;
            }
            return (0);         /* This is dummy */
        }

        /* Make Binary Tree */
        public static void mkbtree()
        {
            uint[] list;
            int n;
            int i;
            double dmy;
            double[] area = new double[6];

            n = -1;
            use_btree = 0;
            area[0] = view.minx;    /* Ray Tracing Area's coordinates */
            area[1] = view.miny;
            area[2] = view.minz;
            area[3] = view.maxx;
            area[4] = view.maxy;
            area[5] = view.maxz;
            //if ((list = (POINTER*)malloc((use_atable + 1) * sizeof(POINTER))) == NULL
            //|| (outprim = (POINTER*)malloc((use_atable + 1) * sizeof(POINTER))) == NULL)
            //{
            //    fprintf(stderr, "*** No memory for allocation. ***\n");
            //    terminate(1);
            //}
            list = new uint[use_atable + 1];
            outprim = new uint[use_atable + 1];
            for (i = 0; i < use_atable; i++) list[i] = (uint)(i + 1);
            list[i] = 0;
            makeoutprim();

            /* call */
            bbtree(n, 0, list, 0, list, area, out dmy);

            /* terminated */
            //free((char*)list);      /* free memory */
        }

        private const int ADD_BTREE = 20000;
        public static void more_btree()
        {
            //uint no;
            //uint i;
            //REGISTER POINTER *sub_btree;

            /* Extending of btree[] */
            Debug.Log("Extending of btree[].\n");
            //no = max_btree + ADD_BTREE;
            //if ((sub_btree = (POINTER*)malloc(no * sizeof(POINTER))) == NULL)
            //{
            //    fprintf(stderr, "*** No memory for allocation. ***\n");
            //    terminate(1);
            //}
            //for (i = 0; i < use_btree; i++) sub_btree[i] = btree[i];
            //free((char*)btree);
            //btree = sub_btree;
            max_atable += ADD_BTREE;
            Array.Resize(ref btree, (int)max_btree);
        }

        /* Time constant */
        private const double TIME_SEARCH_BTREE = (1.1897569e-4);
        private const double TIME_CROSS_POINT = (7.8611583e-4);
        private const double TIME_META_CROSS_POINT = (20 * TIME_CROSS_POINT);

        /* Build a part of btree */
        public static bool bbtree(int n, int list_sub, uint[] l_list_sub, int list, uint[] l_list, double[] area
	   , out double time)
        {
            uint i;
            uint j;
            int flag;
            uint[] l_list1, l_list2;
            int list1, list2;
            uint[] l_temp1, l_temp2;
            int temp1, temp2;
            int a;
            bool check;
            bool ret1, ret2;
            uint cnt;
            uint cnt_prim;
            uint work1, work2;
            uint k;
            double[] area1 = new double[6], area2 = new double[6];
            double time1, time2;
            double time_cost;

            n++;

            time = 0.0;

            /* make list[] */
            check = false;
            cnt_prim = cnt = 0;
            time_cost = 0;
            temp2 = list; l_temp2 = l_list;
            for (temp1 = list_sub, l_temp1 = l_list_sub; l_temp1[temp1] != 0; temp1++)
            {
                flag = FULL;
                i = atable[k = l_temp1[temp1] - 1, 1];
                for (j = atable[k, 0]; j <= i; j++)
                {
                    a = chksolid(j, area[0], area[1], area[2]
                         , area[3], area[4], area[5]);
                    if (a == EMPTY)
                    {
                        flag = EMPTY;
                        break;
                    }
                    if (flag == FULL) flag = a;
                }
                if (flag == FULL && scm[PRIM(i).scm].type != 2)
                {
                    /* Terminal Node Condition I */
                    check = true;
                }
                if (flag != EMPTY)
                {
                    l_temp2[temp2++] = l_temp1[temp1];
                    cnt++;
                    cnt_prim += i - atable[k, 0] + 1;
                    /* Count the number in this and-table. */
                    if (PRIM(i).kind == 5)
                    {
                        time_cost += TIME_META_CROSS_POINT * (i - atable[k, 0] + 1);
                    }
                    else
                    {
                        time_cost += TIME_CROSS_POINT * (i - atable[k, 0] + 1);
                    }
                }
            }
            l_temp2[temp2] = 0;
            if (check || cnt == 0 || n >= view.nmax
            || (n > view.nmin && cnt_prim <= view.pmax))
            {
                /* Terminal Node */
                if (cnt_prim >= 1)
                {
                    Vector3 p1 = new Vector3((float)area[0], (float)area[1], (float)area[2]);
                    Vector3 p2 = new Vector3((float)area[3], (float)area[4], (float)area[5]);
                    //EXRAYcontext.Post(_ =>
                    //{
                    //    EXRAYtracer.MakeBox(p1, p2);
                    //}, null);
                    EXRAYtracer.queueBox.Enqueue(new EXRAYtracer.OneBox(p1, p2, 0.75f * (float)cnt_prim / (float)view.pmax));
                }
            TERMINAL_NODE:
                while (use_btree + cnt + 2 >= max_btree)
                {
                    more_btree();
                }
                btree[use_btree++] = 0; /* Terminal node */
                btree[use_btree++] = cnt;
                for (temp1 = list, l_temp1 = l_list; (i = l_temp1[temp1]) != 0; temp1++)
                {
                    btree[use_btree++] = i - 1;
                }
                time = time_cost + TIME_SEARCH_BTREE;
                return (true);
            }
            /* A */
            work1 = use_btree;
            if (use_btree >= max_btree)
            {
                more_btree();
            }
            btree[use_btree++] = 0;
            //if ((list1 = (POINTER*)malloc((use_atable + 1) * sizeof(POINTER))) == NULL
            //|| (list2 = (POINTER*)malloc((use_atable + 1) * sizeof(POINTER))) == NULL)
            //{
            //    fprintf(stderr, "*** No memory for allocation. ***\n");
            //    terminate(1);
            //}
            l_list1 = new uint[use_atable + 1]; list1 = 0;
            l_list2 = new uint[use_atable + 1]; list2 = 0;
            for (i = 0; i < 6; i++)
            {
                area1[i] = area2[i] = area[i];
            }
            switch (n % 3)
            {
                case XX - 1:
                    area1[3] = area2[0] = (area[0] + area[3]) * 0.5;
                    break;

                case YY - 1:
                    area1[4] = area2[1] = (area[1] + area[4]) * 0.5;
                    break;

                case ZZ - 1:
                    area1[5] = area2[2] = (area[2] + area[5]) * 0.5;
                    break;
            }
            ret1 = bbtree(n, list, l_list, list1, l_list1, area1, out time1);
            work2 = use_btree;
            ret2 = bbtree(n, list, l_list, list2, l_list2, area2, out time2);

            /* C */
            if (!(ret1 && ret2)) goto L_D;
            for (temp1 = list1, l_temp1 = l_list1, temp2 = list2, l_temp2 = l_list2; (i = l_temp1[temp1]) != 0; temp1++)
            {
                if (i != l_temp2[temp2++]) break;
            }
            if (l_temp1[temp1] != 0 || l_temp2[temp2] != 0) goto L_D;

            /* move btree[work1+1] - btree[work2-1] *
             * to btree[work1]   - btree[work2-2]   */
            //temp2 = (temp1 = &btree[work1]) + 1;
            //for (i = work2 - work1 - 1; i > 0; i--) *temp1++ = *temp2++;
            //use_btree = work2 - 1;
            temp1 = (int)work1; l_temp1 = btree;
            temp2 = temp1 + 1; l_temp2 = l_temp1;
            for (i = work2 - work1 - 1; i > 0; i--) l_temp1[temp1++] = l_temp2[temp2++];
            use_btree = work2 - 1;

            for (temp1 = list, l_temp1 = l_list, temp2 = list1, l_temp2 = l_list1; (i = l_temp2[temp2]) != 0; temp2++)
            {
                l_temp1[temp1++] = i;
            }
            l_temp1[temp1] = 0;

            time = time1;

            /* terminated */
            //free((char*)list1);
            //free((char*)list2);
            return (true);

        /* D */
        L_D:
            btree[work1] = work2 - work1;   /* Attention : btree[] style of      *
				         * non-terminal node is changed from *
				         * absolute style to relative one.   */
            /* terminated */
            //free((char*)list1);
            //free((char*)list2);

            /* Binary Tree optimizing */
            if (false && time_cost < (time = 0.75 * (time1 + time2)))
            {
                /* Optimized */
                use_btree = work1;
            //goto TERMINAL_NODE;
            TERMINAL_NODE:
                while (use_btree + cnt + 2 >= max_btree)
                {
                    more_btree();
                }
                btree[use_btree++] = 0; /* Terminal node */
                btree[use_btree++] = cnt;
                for (temp1 = list, l_temp1 = l_list; (i = l_temp1[temp1]) != 0; temp1++)
                {
                    btree[use_btree++] = i - 1;
                }
                time = time_cost + TIME_SEARCH_BTREE;
                return (true);
            }
            time += TIME_SEARCH_BTREE;

            return (false);
        }

        public static void makeoutprim()
        {
            uint i, j;
            double max, min;
            uint top, bottom;

            use_outprim = 0;
            max = INFINITY;
            min = (-INFINITY);

            for (i = 0; i < use_atable; i++)
            {
                top = atable[i, 0];
                bottom = atable[i, 1];
                for (j = top; j <= bottom; j++)
                {
                    if (chksolid(j, min, min, min, view.minx, max, max) == EMPTY)
                    {
                        break;
                    }
                }
                if (j > bottom)
                {
                    outprim[use_outprim++] = i;
                    continue;
                }

                for (j = top; j <= bottom; j++)
                {
                    if (chksolid(j, min, min, min, max, view.miny, max) == EMPTY)
                    {
                        break;
                    }
                }
                if (j > bottom)
                {
                    outprim[use_outprim++] = i;
                    continue;
                }
                for (j = top; j <= bottom; j++)
                {
                    if (chksolid(j, min, min, min, max, max, view.minz) == EMPTY)
                    {
                        break;
                    }
                }
                if (j > bottom)
                {
                    outprim[use_outprim++] = i;
                    continue;
                }
                for (j = top; j <= bottom; j++)
                {
                    if (chksolid(j, view.maxx, min, min, max, max, max) == EMPTY)
                    {
                        break;
                    }
                }
                if (j > bottom)
                {
                    outprim[use_outprim++] = i;
                    continue;
                }
                for (j = top; j <= bottom; j++)
                {
                    if (chksolid(j, min, view.maxy, min, max, max, max) == EMPTY)
                    {
                        break;
                    }
                }
                if (j > bottom)
                {
                    outprim[use_outprim++] = i;
                    continue;
                }
                for (j = top; j <= bottom; j++)
                {
                    if (chksolid(j, min, min, view.maxz, max, max, max) == EMPTY)
                    {
                        break;
                    }
                }
                if (j > bottom)
                {
                    outprim[use_outprim++] = i;
                    continue;
                }
            }
        }

    }
}
