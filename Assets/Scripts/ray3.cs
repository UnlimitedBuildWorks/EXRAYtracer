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
using static EXRAY.ray2;

namespace EXRAY
{
    public class ray3
    {
        private class p0
        {
            public static bool first_flag = true;
            public static double last_x, last_y, last_z;
            public static int last_sgn;
        }
        public static int iof(uint no, double x, double y, double z)
        {
            double lx = 0.0, ly = 0.0, lz = 0.0;
            double sum;
            double a, r;
            uint i;
            double[] p = new double[0];
            tag_prim pr;
            uint[,] l_sub_atable;
            int sub_atable;
            //static int first_flag = TRUE;
            //static double last_x, last_y, last_z;
            //static int last_sgn;

            pr = PRIM(no);
            if (pr.kind != 5)
            {
                lx = x - pr.cx;    /* convert global coordinates to local ones */
                ly = y - pr.cy;
                lz = z - pr.cz;
                p = pr.par;
            }
            switch (pr.kind)
            {
                case 1:         /* CHOKUHOUTAI */
                    for (i = 0; i < 12; i += 4)
                    {
                        if (Math.Abs(p[i] * lx + p[i + 1] * ly + p[i + 2] * lz) > -p[i + 3])
                        {
                            break;
                        }
                    }
                    if (i >= 12)
                    {   /* inside */
                        return ((int)(-SGN(pr.sg)));
                    }
                    /* outside */
                    return ((int)SGN(pr.sg));

                case 2:         /* HEIMEN */
                    return ((int)SGN(pr.sg) * SGN(lx * p[0] + ly * p[1] + lz * p[2]));

                case 3:         /* 2JI KYOKUMEN */
                case 4:
                    return ((int)(SGN(lx * (p[0] * lx + 2 * (p[4] * lz + p[5] * ly))
                              + ly * (p[1] * ly + 2 * p[3] * lz)
                              + p[2] * lz * lz + p[6]) * SGN(pr.sg)));

                case 5:         /* Meta-Ball */
                    if (!p0.first_flag && p0.last_x == x && p0.last_y == y && p0.last_z == z)
                    {
                        /* SGN has been already computed. */
                        return (p0.last_sgn);
                    }
                    p0.first_flag = false;
                    sum = 0.0;
                    l_sub_atable = atable; sub_atable = 0;
                    for (i = use_atable; i > 0; i--, sub_atable++)
                    {
                        if (PRIM(l_sub_atable[sub_atable, 0]).kind != 5)
                        {   /* No Meta-Ball */
                            continue;
                        }
                        pr = PRIM(l_sub_atable[sub_atable, 0]);
                        lx = x - pr.cx;
                        ly = y - pr.cy;
                        lz = z - pr.cz;
                        p = pr.par;
                        r = (p[0] * lx + 2 * (p[4] * lz + p[5] * ly)) * lx
                        + (p[1] * ly + 2 * p[3] * lz) * ly + p[2] * lz * lz;
                        if (r >= p[14])
                        {   /* r >= b (No effect) */
                            continue;
                        }
                        r = Math.Sqrt(r);
                        /* compute f(r) */
                        if (r < p[8])
                        {   /* r < b/3 */
                            a = r * p[9];
                            sum += p[12] * (1.0 - 3 * a * a);
                        }
                        else
                        {       /* b/3 <= r < b */
                            a = 1.0 - r * p[9];
                            sum += a * a * p[13];
                        }
                    }
                    if (sum >= 1.0)
                    {   /* Inside */
                        p0.last_sgn = (-1);
                    }
                    else
                    {       /* Outside */
                        p0.last_sgn = 1;
                    }
                    p0.last_x = x;
                    p0.last_y = y;
                    p0.last_z = z;
                    return (p0.last_sgn);

                default:
                    Debug.LogError("*** Inside error(iof()). ***\n");
                    terminate(1);
                    break;
            }
            return (0);     /* dummy for lint */
        }

        /* local variables */
        private static double vx, vy, vz;
        private static double px, py, pz;
        private static bool primchk;
        private static ulong count;
        private static uint pdno;
        private static CROSS_POINT cp;
        private static bool[] xyzflag = new bool[3];
        private static byte[] status;
        private static byte[] l_sub_status;
        private static int sub_status;
        private static byte check_status = 0;
        private static uint t_scm;
        private static double[] statt;
        private static uint[] statprimno;
        private static double[,] statcross;
        private static double maxt;
        private static bool neg_vx, neg_vy, neg_vz;
        private const double LESS_LIMIT = (1E-20);
        private const double M_LESS_LIMIT = (-LESS_LIMIT);

        private class p1
        {
            public static double[][] x = new double[2][] {
                new double[3], new double[3]
            };
            public static double[][] xx = new double[2][]
            {
                x[0], x[1]
            };
            public static double[][] y = new double[2][] {
                new double[3], new double[3]
            };
            public static double[][] yy = new double[2][]
            {
                y[0], y[1]
            };
            public static double[][] z = new double[2][] {
                new double[3], new double[3]
            };
            public static double[][] zz = new double[2][]
            {
                z[0], z[1]
            };
        }
        public static void btree_cross_point(CROSS_POINT pcp, CROSS_POINT acp)
        {
            int i;
            bool chkflag;
            uint sprimno = 0;
            uint ii;
            double t = 0.0;
            /* static variables */
            //static double x[2][3], y[2][3], z[2][3];
            //static double* xx[2] = {
            //x[0], x[1]
            //};
            //static double* yy[2] = {
            //y[0], y[1]
            //};
            //static double* zz[2] = {
            //z[0], z[1]
            //};
            bool outflag;
            uint primno = 0;
            double chkt = 0.0;
            double tt = 0.0;
            double st;
            double bbx = 0.0, bby = 0.0, bbz = 0.0;
            double sbbx = 0.0, sbby = 0.0, sbbz = 0.0;

            /* clear 'appear_meta_list' */
            appear_meta_list = 0;

            /* copy variables */
            vx = pcp.vx;
            vy = pcp.vy;
            vz = pcp.vz;
            px = pcp.x;
            py = pcp.y;
            pz = pcp.z;
            primchk = pcp.flag;
            count = PRIM(pcp.primno).count;
            pdno = PRIM(pcp.primno).pdno;
            t_scm = pcp.t_scm;
            cp = acp;
            if (px < view.minx || px > view.maxx || py < view.miny || py > view.maxy
            || pz < view.minz || pz > view.maxz)
            {
                outflag = chkflag = true;
            }
            else
            {
                outflag = chkflag = false;
            }

            if (vx < LESS_LIMIT && vx > M_LESS_LIMIT)
            {
                xyzflag[0] = true;
                p1.x[0][0] = view.minx;
                p1.x[1][0] = view.maxx;
                p1.x[0][1] = p1.x[0][2] = p1.x[1][1] = p1.x[1][2] = 0;
            }
            else
            {
                xyzflag[0] = false;
            }

            if (vy < LESS_LIMIT && vy > M_LESS_LIMIT)
            {
                xyzflag[1] = true;
                p1.y[0][1] = view.miny;
                p1.y[1][1] = view.maxy;
                p1.y[0][0] = p1.y[0][2] = p1.y[1][0] = p1.y[1][2] = 0;
            }
            else
            {
                xyzflag[1] = false;
            }

            if (vz < LESS_LIMIT && vz > M_LESS_LIMIT)
            {
                xyzflag[2] = true;
                p1.z[0][2] = view.minz;
                p1.z[1][2] = view.maxz;
                p1.z[0][0] = p1.z[0][1] = p1.z[1][0] = p1.z[1][1] = 0;
            }
            else
            {
                xyzflag[2] = false;
            }

            /* making x[2][3] */
            if (!xyzflag[0])
            {
                t = ((p1.x[0][0] = view.minx) - px) / vx;
                p1.x[0][1] = vy * t + py;
                p1.x[0][2] = vz * t + pz;
                if (chkflag && view.miny <= p1.x[0][1] && p1.x[0][1] <= view.maxy
                    && view.minz <= p1.x[0][2] && p1.x[0][2] <= view.maxz)
                {
                    chkflag = false;
                    chkt = t;
                }
                t = ((p1.x[1][0] = view.maxx) - px) / vx;
                p1.x[1][1] = vy * t + py;
                p1.x[1][2] = vz * t + pz;
                if (chkflag && view.miny <= p1.x[1][1] && p1.x[1][1] <= view.maxy
                    && view.minz <= p1.x[1][2] && p1.x[1][2] <= view.maxz)
                {
                    chkflag = false;
                    chkt = t;
                }
            }

            /* making y[2][3] */
            if (!xyzflag[1])
            {
                t = ((p1.y[0][1] = view.miny) - py) / vy;
                p1.y[0][0] = vx * t + px;
                p1.y[0][2] = vz * t + pz;
                if (chkflag && view.minx <= p1.y[0][0] && p1.y[0][0] <= view.maxx
                    && view.minz <= p1.y[0][2] && p1.y[0][2] <= view.maxz)
                {
                    chkflag = false;
                    chkt = t;
                }
                t = ((p1.y[1][1] = view.maxy) - py) / vy;
                p1.y[1][0] = vx * t + px;
                p1.y[1][2] = vz * t + pz;
                if (chkflag && view.minx <= p1.y[1][0] && p1.y[1][0] <= view.maxx
                    && view.minz <= p1.y[1][2] && p1.y[1][2] <= view.maxz)
                {
                    chkflag = false;
                    chkt = t;
                }
            }

            /* making z[2][3] */
            if (!xyzflag[2])
            {
                t = ((p1.z[0][2] = view.minz) - pz) / vz;
                p1.z[0][0] = vx * t + px;
                p1.z[0][1] = vy * t + py;
                if (chkflag && view.minx <= p1.z[0][0] && p1.z[0][0] <= view.maxx
                    && view.miny <= p1.z[0][1] && p1.z[0][1] <= view.maxy)
                {
                    chkflag = false;
                    chkt = t;
                }
                t = ((p1.z[1][2] = view.maxz) - pz) / vz;
                p1.z[1][0] = vx * t + px;
                p1.z[1][1] = vy * t + py;
            }

            if (++check_status == 0 || check_status == 1)
            {
                l_sub_status = status; sub_status = 0;
                for (i = (int)use_atable; i > 0; i--)
                {
                    l_sub_status[sub_status++] = 0;
                }
                check_status = 1;
            }
            neg_vx = NEGATIVE(vx);
            neg_vy = NEGATIVE(vy);
            neg_vz = NEGATIVE(vz);

            /* search btree[] */
            st = maxt = 2 * INFINITY;
            if (!outflag)
            {       /* IN */
                if (search_btree((int)(-1), 0, btree, p1.xx, p1.yy, p1.zz))
                {
                    meta_cross_point();
                    return;
                }
            }

            /* Check of outsider primitives. */
            for (ii = 0; ii < use_outprim; ii++)
            {
                if (cross_point(outprim[ii], ref tt, ref primno, true
                            , ref bbx, ref bby, ref bbz))
                {
                    st = tt;
                    sprimno = primno;
                    sbbx = bbx;
                    sbby = bby;
                    sbbz = bbz;
                }
            }
            if (outflag && !chkflag && chkt >= 0)
            {
                if (search_btree((int)(-1), 0, btree, p1.xx, p1.yy, p1.zz))
                {
                    meta_cross_point();
                    return;
                }
            }
            if (st >= INFINITY)
            {
                cp.flag = false;
                meta_cross_point();
                return;
            }
            cp.flag = true;
            cp.t = st;
            cp.primno = sprimno;
            cp.x = sbbx;
            cp.y = sbby;
            cp.z = sbbz;
            meta_cross_point();
        }

        /* Initializing of cross_point() */
        public static void initialize_cross_point()
        {
            //status = malloc((use_atable + 1) * sizeof(char));
            status = new byte[use_atable + 1];
            //statt = (double*)malloc((use_atable + 1) * sizeof(double));
            statt = new double[use_atable + 1];
            //statprimno = (POINTER*)malloc((use_atable + 1) * sizeof(POINTER));
            statprimno = new uint[use_atable + 1];
            //statcross = (double(*)[3])malloc((use_atable + 1) * 3 * sizeof(double));
            statcross = new double[use_atable + 1, 3];
            //if (status == NULL || statt == NULL
            //|| statprimno == NULL || statcross == NULL)
            //{
            //    fprintf(stderr, "*** No memory for allocation. ***\n");
            //    terminate(1);
            //}
        }

        public static bool search_btree(int n, int bt, uint[] l_bt, double[][] x, double[][] y, double[][] z)
        {
            uint i;
            bool work;
            bool cond1, cond2, cond3, cond4;
            uint primno = 0, sprimno = 0;
            double st, t = 0.0;
            double[][] s = new double[2][];
            double[] w = new double[3];
            double bbx = 0.0, bby = 0.0, bbz = 0.0;
            double sbbx = 0.0, sbby = 0.0, sbbz = 0.0;

            if (++n >= 3)
            {       /* n = n % 3 */
                n = 0;
            }

            if (l_bt[bt] == 0)
            {       /* Terminal node */
                st = 2 * INFINITY;
                for (i = l_bt[++bt]; i > 0; i--)
                {
                    if (cross_point(l_bt[++bt], ref t, ref primno, true
                            , ref bbx, ref bby, ref bbz))
                    {
                        /* it is cross point */
                        st = t;
                        sprimno = primno;
                        sbbx = bbx;
                        sbby = bby;
                        sbbz = bbz;
                    }
                }
                if (st >= INFINITY || (sbbx < x[0][0] || sbbx > x[1][0]
                               || sbby < y[0][1] || sbby > y[1][1]
                               || sbbz < z[0][2] || sbbz > z[1][2]))
                {
                    /* no cross point */
                    return (false);
                }
                cp.flag = true;
                cp.t = st;
                cp.primno = sprimno;
                cp.x = sbbx;
                cp.y = sbby;
                cp.z = sbbz;
                return (true);
            }

            /* Non terminal node */
            switch (n)
            {
                case XX - 1:        /* divide X axis */
                    w[0] = (x[0][0] + x[1][0]) * 0.5;
                    w[1] = (x[0][1] + x[1][1]) * 0.5;
                    w[2] = (x[0][2] + x[1][2]) * 0.5;
                    if (xyzflag[0])
                    {
                        if (px < w[0]) goto L_X1;
                        else
                        {
                            goto L_X2;
                        L_X2:
                            s[0] = w;
                            s[1] = x[1];
                            //return (search_btree(n, bt + *bt, s, y, z));
                            return (search_btree(n, (int)(bt + l_bt[bt]), l_bt, s, y, z));
                        }
                    }
                    if ((cond1 = w[1] < y[0][1]) | (cond2 = w[1] > y[1][1])
                        | (cond3 = w[2] < z[0][2]) | (cond4 = w[2] > z[1][2]))
                    {
                        /* Attention : You may not change 4      *
                         * '|'(arithmetical OR)s in this line to *
                         * '||'(logical OR)s.                    */
                        if ((cond1 && x[0][1] < y[0][1]) || (cond2 && x[0][1] > y[1][1])
                        || (cond3 && x[0][2] < z[0][2]) || (cond4 && x[0][2] > z[1][2]))
                        {
                            goto L_X2;
                        L_X2:
                            s[0] = w;
                            s[1] = x[1];
                            //return (search_btree(n, bt + *bt, s, y, z));
                            return (search_btree(n, (int)(bt + l_bt[bt]), l_bt, s, y, z));
                        }
                        else
                        {
                            goto L_X1;
                        }
                    }
                    if (!((work = NEGATIVE(px - w[0])) ^ neg_vx))
                    {
                        if (!work)
                        {
                            goto L_X2;
                        L_X2:
                            s[0] = w;
                            s[1] = x[1];
                            //return (search_btree(n, bt + *bt, s, y, z));
                            return (search_btree(n, (int)(bt + l_bt[bt]), l_bt, s, y, z));
                        }
                        else goto L_X1;
                    }
                    /* both divided rectangles will be checked */
                    if (work)
                    {
                        s[0] = x[0];
                        s[1] = w;
                        if (search_btree(n, bt + 1, l_bt, s, y, z))
                        {
                            return (true);
                        }
                    L_X2:
                        s[0] = w;
                        s[1] = x[1];
                        //return (search_btree(n, bt + *bt, s, y, z));
                        return (search_btree(n, (int)(bt + l_bt[bt]), l_bt, s, y, z));
                    }
                    s[0] = w;
                    s[1] = x[1];
                    //if (search_btree(n, bt + *bt, s, y, z))
                    if (search_btree(n, (int)(bt + l_bt[bt]), l_bt, s, y, z))
                    {
                        return (true);
                    }
                L_X1:
                    s[0] = x[0];
                    s[1] = w;
                    //return (search_btree(n, bt + 1, s, y, z));
                    return (search_btree(n, bt + 1, l_bt, s, y, z));

                case YY - 1:        /* divide Y axis */
                    w[0] = (y[0][0] + y[1][0]) * 0.5;
                    w[1] = (y[0][1] + y[1][1]) * 0.5;
                    w[2] = (y[0][2] + y[1][2]) * 0.5;
                    if (xyzflag[1])
                    {
                        if (py < w[1]) goto L_Y1;
                        else
                        {
                            goto L_Y2;
                        L_Y2:
                            s[0] = w;
                            s[1] = y[1];
                            //return (search_btree(n, bt + *bt, x, s, z));
                            return (search_btree(n, (int)(bt + l_bt[bt]), l_bt, x, s, z));
                        }
                    }
                    if ((cond1 = w[0] < x[0][0]) | (cond2 = w[0] > x[1][0])
                        | (cond3 = w[2] < z[0][2]) | (cond4 = w[2] > z[1][2]))
                    {
                        /* Attention : You may not change 4      *
                         * '|'(arithmetical OR)s in this line to *
                         * '||'(logical OR)s.                    */
                        if ((cond1 && y[0][0] < x[0][0]) || (cond2 && y[0][0] > x[1][0])
                        || (cond3 && y[0][2] < z[0][2]) || (cond4 && y[0][2] > z[1][2]))
                        {
                            goto L_Y2;
                        L_Y2:
                            s[0] = w;
                            s[1] = y[1];
                            //return (search_btree(n, bt + *bt, x, s, z));
                            return (search_btree(n, (int)(bt + l_bt[bt]), l_bt, x, s, z));
                        }
                        else
                        {
                            goto L_Y1;
                        }
                    }
                    if (!((work = NEGATIVE(py - w[1])) ^ neg_vy))
                    {
                        if (!work)
                        {
                            goto L_Y2;
                        L_Y2:
                            s[0] = w;
                            s[1] = y[1];
                            //return (search_btree(n, bt + *bt, x, s, z));
                            return (search_btree(n, (int)(bt + l_bt[bt]), l_bt, x, s, z));
                        }
                        else goto L_Y1;
                    }
                    /* both divided rectangles will be checked */
                    if (work)
                    {
                        s[0] = y[0];
                        s[1] = w;
                        //if (search_btree(n, bt + 1, x, s, z))
                        if (search_btree(n, bt + 1, l_bt, x, s, z))
                        {
                            return (true);
                        }
                    L_Y2:
                        s[0] = w;
                        s[1] = y[1];
                        //return (search_btree(n, bt + *bt, x, s, z));
                        return (search_btree(n, (int)(bt + l_bt[bt]), l_bt, x, s, z));
                    }
                    s[0] = w;
                    s[1] = y[1];
                    //if (search_btree(n, bt + *bt, x, s, z))
                    if (search_btree(n, (int)(bt + l_bt[bt]), l_bt, x, s, z))
                    {
                        return (true);
                    }
                L_Y1:
                    s[0] = y[0];
                    s[1] = w;
                    return (search_btree(n, bt + 1, l_bt, x, s, z));

                case ZZ - 1:        /* divide Z axis */
                    w[0] = (z[0][0] + z[1][0]) * 0.5;
                    w[1] = (z[0][1] + z[1][1]) * 0.5;
                    w[2] = (z[0][2] + z[1][2]) * 0.5;
                    if (xyzflag[2])
                    {
                        if (pz < w[2]) goto L_Z1;
                        else
                        {
                            goto L_Z2;
                        L_Z2:
                            s[0] = w;
                            s[1] = z[1];
                            //return (search_btree(n, bt + *bt, x, y, s));
                            return (search_btree(n, (int)(bt + l_bt[bt]), l_bt, x, y, s));
                        }
                    }
                    if ((cond1 = w[0] < x[0][0]) | (cond2 = w[0] > x[1][0])
                        | (cond3 = w[1] < y[0][1]) | (cond4 = w[1] > y[1][1]))
                    {
                        /* Attention : You may not change 4      *
                         * '|'(arithmetical OR)s in this line to *
                         * '||'(logical OR)s.                    */
                        if ((cond1 && z[0][0] < x[0][0]) || (cond2 && z[0][0] > x[1][0])
                        || (cond3 && z[0][1] < y[0][1]) || (cond4 && z[0][1] > y[1][1]))
                        {
                            goto L_Z2;
                        L_Z2:
                            s[0] = w;
                            s[1] = z[1];
                            //return (search_btree(n, bt + *bt, x, y, s));
                            return (search_btree(n, (int)(bt + l_bt[bt]), l_bt, x, y, s));
                        }
                        else
                        {
                            goto L_Z1;
                        }
                    }
                    if (!((work = NEGATIVE(pz - w[2])) ^ neg_vz))
                    {
                        if (!work)
                        {
                            goto L_Z2;
                        L_Z2:
                            s[0] = w;
                            s[1] = z[1];
                            //return (search_btree(n, bt + *bt, x, y, s));
                            return (search_btree(n, (int)(bt + l_bt[bt]), l_bt, x, y, s));
                        }
                        else goto L_Z1;
                    }
                    /* both divided rectangles will be checked */
                    if (work)
                    {
                        s[0] = z[0];
                        s[1] = w;
                        if (search_btree(n, bt + 1, l_bt, x, y, s))
                        {
                            return (true);
                        }
                    L_Z2:
                        s[0] = w;
                        s[1] = z[1];
                        //return (search_btree(n, bt + *bt, x, y, s));
                        return (search_btree(n, (int)(bt + l_bt[bt]), l_bt, x, y, s));
                    }
                    s[0] = w;
                    s[1] = z[1];
                    if (search_btree(n, (int)(bt + l_bt[bt]), l_bt, x, y, s))
                    {
                        return (true);
                    }
                L_Z1:
                    s[0] = z[0];
                    s[1] = w;
                    return (search_btree(n, bt + 1, l_bt, x, y, s));
            }
            return (false);         /* This line is dummy for lint. */
        }

        public static double siki(double t)
        {
            uint i;
            double[] p;
            uint[] l_sub_meta_list = meta_list;
            int sub_meta_list = 0;
            double sum, r, a, tt;
            //REGISTER_DOUBLE(sum);
            //REGISTER_DOUBLE(r);
            //REGISTER_DOUBLE(a);
            //REGISTER_DOUBLE(tt);

            tt = t;
            sum = (-1);
            for (i = use_meta_list; i > 0; i--)
            {
                p = PRIM(l_sub_meta_list[sub_meta_list++]).par;
                if ((r = (p[17] * tt + 2 * p[18]) * tt + p[19]) >= p[14])
                {
                    continue;
                }
                r = Math.Sqrt(r);

                /* compute f(r) */
                if (r < p[8])
                {
                    a = r * p[9];
                    sum += p[12] * (1.0 - 3 * a * a);
                }
                else
                {
                    a = 1.0 - r * p[9];
                    sum += a * a * p[13];
                }
            }
            return (sum);
        }

        public static double siki_differential(double t, out double differential)
        {
            uint i;
            double[] p;
            uint[] l_sub_meta_list = meta_list;
            int sub_meta_list = 0;
            double sum, r, a;
            //REGISTER_DOUBLE(sum);
            //REGISTER_DOUBLE(r);
            //REGISTER_DOUBLE(a);

            sum = (-1);
            differential = 0;
            for (i = use_meta_list; i > 0; i--)
            {
                p = PRIM(l_sub_meta_list[sub_meta_list++]).par;
                if ((r = (p[17] * t + 2 * p[18]) * t + p[19]) >= p[14])
                {
                    continue;
                }
                r = Math.Sqrt(r);

                /* compute f(r) */
                if (r < p[8])
                {
                    a = r * p[9];
                    sum += p[12] * (1.0 - 3 * a * a);
                    differential -= p[10] * (p[17] * t + p[18]);
                    /* df(r)/dt = -6*W/(b*b)*(a*t+b) */
                }
                else
                {
                    a = 1.0 - r * p[9];
                    sum += a * a * p[13];
                    differential += p[11] * (p[9] - 1 / r) * (p[17] * t + p[18]);
                    /* df(r)/dt = 3*W/b*(1/b-1/r)*(a*t+b) */
                }
            }
            return (sum);
        }

        public static void siki_sub(ref uint no, double t)
        {
            uint i;
            double[] p;
            uint[] l_sub_meta_list = meta_list;
            int sub_meta_list = 0;
            double r, a, max;
            //REGISTER_DOUBLE(r);
            //REGISTER_DOUBLE(a);
            //REGISTER_DOUBLE(max);

            max = 0;
            for (i = use_meta_list; i > 0; i--, sub_meta_list++)
            {
                p = PRIM(l_sub_meta_list[sub_meta_list]).par;
                if ((r = (p[17] * t + 2 * p[18]) * t + p[19]) >= p[14])
                {
                    continue;
                }
                r = Math.Sqrt(r);

                /* compute f(r) */
                if (r < p[8])
                {
                    a = r * p[9];
                    a = p[12] * (1.0 - 3 * a * a);
                }
                else
                {
                    a = 1.0 - r * p[9];
                    a *= a * p[13];
                }
                if (a > max)
                {
                    max = a;
                    no = l_sub_meta_list[sub_meta_list];
                }
            }
        }

        public static void meta_cross_point()
        {
            uint i;
            uint j;
            double[] p;
            uint use_s1;
            double x, y, z;
            bool iov;
            //REGISTER double(*swap_stack)[2];
            double[,] swap_stack;
            tag_prim tprim;
            double wt, fwt;
            int solno;
            int h_solno = 0;
            uint primno = 0;
            uint use_s2, use_s3;
            uint k = 0;
            uint h_primno = 0;
            double mind, maxd, vmind, vmaxd;
            double ot1, ot2, swap, fot1, fot2;
            double ot1s, ot2s;
            double d;
            double it1, it2, it3, fit1, fit2, fit3;
            double midd, vmidd;
            double d1, d2, d3;
            double sol1, sol2;
            double bx = 0.0, by = 0.0, bz = 0.0;
            double l_mind, l_vmind;
            double limit_stack3;
            double sol = 0.0;
            double h_sol = 0.0;
            double h_bx = 0.0, h_by = 0.0, h_bz = 0.0;
            double differential;
            double e;

            /* check meta-ball's number */
            if (appear_meta_list == 0) return;

            /* compute alpha beta gamma */
            j = appear_meta_list;
            use_meta_list = 0;
            for (i = 0; i < use_atable && j > 0; i++)
            {
                if (status[i] != check_status
                    || (tprim = PRIM(k = atable[i, 0])).kind != 5)
                {
                    continue;
                }
                appear_meta_list--;
                p = tprim.par;
                x = px - tprim.cx;
                y = py - tprim.cy;
                z = pz - tprim.cz;
                p[17] = (p[0] * vx + 2 * (p[4] * vz + p[5] * vy)) * vx
                    + (p[1] * vy + 2 * p[3] * vz) * vy + p[2] * vz * vz;
                p[18] = (p[0] * x + p[4] * z + p[5] * y) * vx
                + (p[1] * y + p[3] * z + p[5] * x) * vy
                + (p[2] * z + p[3] * y + p[4] * x) * vz;
                if ((p[19] = (p[0] * x + 2 * (p[4] * z + p[5] * y)) * x + (p[1] * y + 2 * p[3] * z) * y + p[2] * z * z) == 0)
                {

                    p[19] = 1 / INFINITY;
                }
                if ((p[15] = p[18] * p[18] - p[17] * (p[16] = p[19] + p[6])) <= 0)
                {
                    continue;       /* No effect */
                }
                meta_list[use_meta_list++] = k;
            }
            if (use_meta_list == 0) return;

            /* compute IOF */
            iov = true;         /* If IOF is negative,then iov is TRUE. */

            /* stacks clear */
            use_s1 = use_s2 = 0;
            stack3[0, 0] = 2 * INFINITY;
            use_s3 = 1;

            for (i = 0; i < use_meta_list; i++)
            {
                p = PRIM(meta_list[i]).par;
                /* calculate outside */
                if (p[17] == 0) continue;
                d = Math.Sqrt(p[15]);
                if (p[18] > 0)
                {
                    ot1 = -(p[18] + d) / p[17];
                }
                else
                {
                    ot1 = (d - p[18]) / p[17];
                }
                ot2 = p[16] / (p[17] * ot1);
                if (ot1 >= ot2)
                {   /* swap ot1 and ot2 */
                    swap = ot1;
                    ot1 = ot2;
                    ot2 = swap;
                }
                if (ot2 <= 0) goto CHECK;
                /* calculate inside */
                d = p[18] * p[18] - p[17] * (e = p[19] - 0.99);
                if (d > 0)
                {
                    d = Math.Sqrt(d);
                    if (p[18] > 0)
                    {
                        it1 = -(p[18] + d) / p[17];
                    }
                    else
                    {
                        it1 = (d - p[18]) / p[17];
                    }
                    it2 = e / (p[17] * it1);
                    if (it1 >= it2)
                    {
                        swap = it1;
                        it1 = it2;
                        it2 = swap;
                    }
                    fit1 = fit3 = siki(it3 = it1);
                    fit2 = siki(it2);
                    if (p[12] > 0 && fit1 < 0 && fit2 < 0)
                    {
                        /* positive Meta-Ball and fit1 < 0 and fit2 < 0 */
                        for (k = 0; k < 10; k++)
                        {
                            if ((fit3 = siki_differential((it3 = 0.5 * (it1 + it2)), out differential)) > 0)
                            {
                                break;
                            }
                            if (differential >= 0)
                            {
                                it1 = it3;
                                fit1 = fit3;
                            }
                            else
                            {
                                it2 = it3;
                                fit2 = fit3;
                            }
                        }
                    }
                }
                else
                {
                    it1 = it2 = it3 = 0.5 * (ot1 + ot2);
                    fit1 = fit2 = fit3 = siki(it1);
                }

                /* flow chart of C */
                if (ot1 > 0)
                {
                    /* Push ot1 */
                    if (NEGATIVE(fot1 = siki(ot1)) ^ iov)
                    {
                        stack3[use_s3, 0] = ot1;
                        stack3[use_s3++, 1] = fot1;
                    }
                    else
                    {
                        stack1[use_s1, 0] = ot1;
                        stack1[use_s1++, 1] = fot1;
                    }
                }
                if (it1 > 0)
                {
                    /* Push it1 */
                    if (NEGATIVE(fit1) ^ iov)
                    {
                        stack3[use_s3, 0] = it1;
                        stack3[use_s3++, 1] = fit1;
                    }
                    else
                    {
                        stack1[use_s1, 0] = it1;
                        stack1[use_s1++, 1] = fit1;
                    }
                }
                if (it2 > 0 && it1 != it2)
                {
                    /* Push it2 */
                    if (NEGATIVE(fit2) ^ iov)
                    {
                        stack3[use_s3, 0] = it2;
                        stack3[use_s3++, 1] = fit2;
                    }
                    else
                    {
                        stack1[use_s1, 0] = it2;
                        stack1[use_s1++, 1] = fit2;
                    }
                }
                if (it3 > 0 && it1 != it3 && it2 != it3)
                {
                    /* Push it3 */
                    if (NEGATIVE(fit3) ^ iov)
                    {
                        stack3[use_s3, 0] = it3;
                        stack3[use_s3++, 1] = fit3;
                    }
                    else
                    {
                        stack1[use_s1, 0] = it3;
                        stack1[use_s1++, 1] = fit3;
                    }
                }
                /* Push ot2 */
                if (NEGATIVE(fot2 = siki(ot2)) ^ iov)
                {
                    stack3[use_s3, 0] = ot2;
                    stack3[use_s3++, 1] = fot2;
                }
                else
                {
                    stack1[use_s1, 0] = ot2;
                    stack1[use_s1++, 1] = fot2;
                }

            CHECK:
                for (j = 0; j < use_s2; j++)
                {
                    ot1s = stack2[j, 0];
                    ot2s = stack2[j, 1];
                    if (ot1 < ot1s && ot1s < ot2 && ot2 < ot2s)
                    {
                        wt = 0.5 * (ot1s + ot2);
                        if (wt > 0)
                        {
                            if (NEGATIVE(fwt = siki(wt)) ^ iov)
                            {
                                stack3[use_s3, 0] = wt;
                                stack3[use_s3++, 1] = fwt;
                            }
                            else
                            {
                                stack1[use_s1, 0] = wt;
                                stack1[use_s1++, 1] = fwt;
                            }
                        }
                        continue;
                    }
                    if (ot1s < ot1 && ot1 < ot2s && ot2s < ot2)
                    {
                        wt = 0.5 * (ot1 + ot2s);
                        if (wt > 0)
                        {
                            if (NEGATIVE(fwt = siki(wt)) ^ iov)
                            {
                                stack3[use_s3, 0] = wt;
                                stack3[use_s3++, 1] = fwt;
                            }
                            else
                            {
                                stack1[use_s1, 0] = wt;
                                stack1[use_s1++, 1] = fwt;
                            }
                        }
                    }
                }
                stack2[use_s2, 0] = ot1;
                stack2[use_s2++, 1] = ot2;
            }

        SECOND_SOLUTION:
            limit_stack3 = mind = 0;
            if (iov) vmind = -2 * INFINITY;
            else vmind = 2 * INFINITY;

            NEXT_ROOT:          /* Calculate roots */

            l_mind = mind;
            l_vmind = vmind;
            mind = 2 * INFINITY;
            for (j = 0; j < use_s3; j++)
            {
                if (stack3[j, 0] > limit_stack3 && stack3[j, 0] < mind)
                {
                    k = j;
                    mind = stack3[j, 0];
                    vmind = stack3[j, 1];
                }
            }
            if (mind >= INFINITY)
            {
                solno = 0;
                goto EXIT;
            }
            limit_stack3 = stack3[k, 0];

            maxd = l_mind;
            vmaxd = l_vmind;
            for (j = 0; j < use_s1; j++)
            {
                if (stack1[j, 0] > maxd && stack1[j, 0] < mind)
                {
                    maxd = stack1[j, 0];
                    vmaxd = stack1[j, 1];
                }
            }
            if (maxd > 0 && maxd <= l_mind) goto NEXT_ROOT;

            /* flow chart of CALROOT */
            /* divide the Range(maxd, mind) */
            vmidd = siki((midd = 0.5 * (maxd + mind)));
            if (!(NEGATIVE(vmaxd) ^ NEGATIVE(vmidd)))
            {
                maxd = midd;
                vmaxd = vmidd;
            }
            else
            {
                mind = midd;
                vmind = vmidd;
            }

            /* divide the Range(maxd, mind) */
            vmidd = siki((midd = 0.5 * (maxd + mind)));
            if (!(NEGATIVE(vmaxd) ^ NEGATIVE(vmidd)))
            {
                maxd = midd;
                vmaxd = vmidd;
                if (vmind <= (-INFINITY) || vmind >= INFINITY)
                {
                    vmind = siki(mind);
                }
            }
            else
            {
                mind = midd;
                vmind = vmidd;
                if (vmaxd <= (-INFINITY) || vmaxd >= INFINITY)
                {
                    vmaxd = siki(maxd);
                }
            }

            /* 2JI KANSU KINJI */
            vmidd = siki((midd = 0.5 * (maxd + mind)));
            d1 = (mind - maxd) * (vmind - vmidd) - (mind - midd) * (vmind - vmaxd);
            d2 = ((mind + midd) * (mind - midd) * (vmind - vmaxd)
              - (mind + maxd) * (mind - maxd) * (vmind - vmidd)) * 0.5;
            d3 = maxd * (midd * (midd - maxd) * vmind - mind * (mind - maxd) * vmidd)
                + mind * midd * (mind - midd) * vmaxd;
            if ((d = d2 * d2 - d1 * d3) < 0)
            {
                d = (-d);
            }
            d = Math.Sqrt(d);
            solno = 1;
            if (d2 > 0) sol1 = -(d2 + d) / d1;
            else sol1 = (d - d2) / d1;
            sol2 = d3 / (d1 * sol1);
            if (sol1 >= maxd && sol1 <= mind)
            {
                sol = sol1;
            }
            else
            {
                if (sol2 >= maxd && sol2 <= mind)
                {
                    sol = sol2;
                }
                else
                {
                    goto NEXT_ROOT;
                }
            }
            if (sol >= maxt)
            {
                solno = 0;
                goto EXIT;
            }

            for (; (solno) > 0; (solno)--)
            {
                bx = vx * sol + px;
                by = vy * sol + py;
                bz = vz * sol + pz;
                siki_sub(ref primno, (double)sol);
                if (scm[PRIM(primno).scm].type != 2 || (!view.trans)
                    || t_scm > use_scm)
                {
                    break;
                }
                for (j = 0; j < use_trans; j++)
                {
                    if (PRIM(k = atable[trans[j], 0]).kind == 5)
                    {   /* Meta-Ball */
                        continue;
                    }
                    for (; k <= atable[trans[j], 1]; k++)
                    {
                        if (iof(k, bx, by, bz) > 0) break;
                    }
                    if (k > atable[trans[j], 1]) break;
                }
                if (j >= use_trans) break;
            }
            if (solno <= 0) goto NEXT_ROOT;

            EXIT:
            if (t_scm < use_scm)
            {
                if (iov)
                {       /* first time */
                    if (solno == 0 || scm[PRIM(primno).scm].type == 2)
                    {
                        /* save data */
                        h_solno = 0;
                    }
                    else
                    {
                        h_solno = solno;
                        h_sol = sol;
                        h_bx = bx;
                        h_by = by;
                        h_bz = bz;
                        h_primno = primno;
                    }
                    iov = true;            /* Inside */
                    i = use_s1;     /* swap use_s1 and use_s3 */
                    use_s1 = use_s3;
                    use_s3 = i;
                    swap_stack = stack1;        /* swap stack1 and stack3 */
                    stack1 = stack3;
                    stack3 = swap_stack;
                    goto SECOND_SOLUTION;
                }
                /* second time */
                if ((solno != 0 && h_solno != 0 && h_sol < sol)
                    || (solno == 0 && h_solno != 0))
                {
                    /* copy */
                    solno = h_solno;
                    sol = h_sol;
                    bx = h_bx;
                    by = h_by;
                    bz = h_bz;
                    primno = h_primno;
                }
            }
            if (solno != 0)
            {       /* solution */
                cp.flag = true;
                cp.t = sol;
                cp.primno = primno;
                cp.x = bx;
                cp.y = by;
                cp.z = bz;
            }
        }

        public static bool cross_point(uint ano, ref double t, ref uint primno, bool check
        , ref double bbx, ref double bby, ref double bbz)
        {
            uint pno;
            uint j;
            bool works;
            double[] p;
            double a, b, c;
            double bx, by, bz;
            bool transchk;
            int solno = 0;
            bool solflag;
            uint transno = 0;
            uint k;
            uint top, bottom;
            uint end;
            double r1, r2, w1, w2;
            double work;
            double sol1 = 0.0, sol2 = 0.0;
            double d, e, f;
            double ppa, ppb, cond;

            if (check)
            {
                if (status[ano] == check_status)
                {   /* it has already checked. */
                    primno = statprimno[ano];
                    bbx = statcross[ano, 0];
                    bby = statcross[ano, 1];
                    bbz = statcross[ano, 2];
                    if ((t = statt[ano]) <= maxt)
                    {
                        return (true);
                    }
                    return (false);
                }
                statt[ano] = 3 * INFINITY;
            }
            status[ano] = check_status;
            if (PRIM(atable[ano, 0]).kind == 5)
            {   /* Meta-Ball */
                appear_meta_list++;
                return (false);
            }
            if (scm[PRIM(atable[ano, 0]).scm].type != 2
            || (!view.trans) || t_scm >= use_scm)
            {
                transchk = false;
            }
            else
            {           /* transparent primitive check has been done. */
                transchk = true;
                for (transno = 0; transno < use_trans; transno++)
                {
                    if (trans[transno] == ano) break;
                }
            }
            top = atable[ano, 0];
            bottom = atable[ano, 1];
            solflag = false;
            for (pno = top; pno <= bottom; pno++)
            {
                /* compute cross points of PRIM(pno) */
                bx = px - PRIM(pno).cx; /* convert global coordinates */
                by = py - PRIM(pno).cy; /*       to local ones        */
                bz = pz - PRIM(pno).cz;
                p = PRIM(pno).par;
                switch (PRIM(pno).kind)
                {
                    case 1:         /* CHOKUHOUTAI */
                        r1 = -INFINITY;
                        r2 = INFINITY;
                        for (j = 0; j < 12; j += 4)
                        {
                            b = p[j] * bx + p[j + 1] * by + p[j + 2] * bz;
                            if ((a = p[j] * vx + p[j + 1] * vy + p[j + 2] * vz) == 0
                                && Math.Abs(b) > -p[j + 3])
                            {
                                solno = 0;
                                goto L_ESWITCH;
                            }
                            if (a != 0)
                            {
                                w1 = -(b - p[j + 3]) / a;
                                w2 = -(b + p[j + 3]) / a;
                                if (w1 > w2)
                                {
                                    work = w1;
                                    w1 = w2;
                                    w2 = work;
                                }
                                /* [r1,r2] AND [w1,w2] */
                                if (r2 <= w1 || w2 <= r1)
                                {
                                    solno = 0;
                                    goto L_ESWITCH;
                                }
                                if (w1 > r1) r1 = w1;
                                if (w2 < r2) r2 = w2;
                            }
                        }
                        if (r1 == r2)
                        {
                            solno = 0;
                            break;
                        }
                        solno = 2;
                        sol1 = r1;
                        sol2 = r2;
                        break;

                    case 2:         /* HEIMEN */
                        if ((a = p[0] * vx + p[1] * vy + p[2] * vz) == 0)
                        {
                            solno = 0;
                            break;
                        }
                        b = p[0] * bx + p[1] * by + p[2] * bz;
                        solno = 2;
                        sol1 = -b / a;
                        if (a <= 0)
                        {
                            sol2 = 2 * maxt;
                        }
                        else
                        {
                            sol2 = sol1;
                            sol1 = (-2 * INFINITY);
                        }
                        break;

                    case 3:         /* 2JI KYOKUMEN */
                    case 4:
                        a = p[0];
                        b = p[1];
                        c = p[2];
                        d = p[3];
                        e = p[4];
                        f = p[5];
                        ppa = (a * vx + 2 * f * vy) * vx + b * vy * vy
                        + (c * vz + 2 * (d * vy + e * vx)) * vz;
                        ppb = (a * bx + e * bz + f * by) * vx
                        + (b * by + d * bz + f * bx) * vy
                            + (c * bz + d * by + e * bx) * vz;
                        c = (a * bx + 2 * f * by) * bx + b * by * by
                        + (c * bz + 2 * (d * by + e * bx)) * bz + p[6];
                        a = ppa;
                        b = ppb;
                        if (a == 0 && b != 0)
                        {
                            solno = 2;
                            sol1 = -c / (2 * b);
                            if (b <= 0)
                            {
                                sol2 = 2 * maxt;
                            }
                            else
                            {
                                sol2 = sol1;
                                sol1 = (-2 * INFINITY);
                            }
                            break;
                        }
                        if ((cond = b * b - a * c) <= 0)
                        {
                            solno = 0;
                            break;
                        }
                        cond = Math.Sqrt(cond);
                        solno = 2;
                        if (b > 0) sol1 = -(b + cond) / a;
                        else sol1 = (cond - b) / a;
                        sol2 = c / (a * sol1);
                        if (sol1 > sol2)
                        {
                            work = sol1;
                            sol1 = sol2;
                            sol2 = work;
                        }
                        if (a < 0)
                        {
                            work = sol1;
                            sol1 = sol2;
                            sol2 = work;
                        }
                        break;

                    default:
                        Debug.LogError("*** Inside error(cross_point()). ***\n");
                        terminate(1);
                        break;
                }
            L_ESWITCH:
                if (solno <= 0) continue;

                /* same primitive check */
                if (t_scm < use_scm && scm[PRIM(pno).scm].type == 2)
                {
                    works = true;   /* Negative */
                }
                else
                {
                    works = false;  /* Positive */
                }
                if (works ^ NEGATIVE(PRIM(pno).sg))
                {
                    sol1 = sol2;
                }
                solno--;

                /* solution range check */
                if (sol1 <= 0 || sol1 >= maxt)
                {
                    solno--;
                }
                for (; solno > 0; solno--)
                {
                    bx = vx * sol1 + px;
                    by = vy * sol1 + py;
                    bz = vz * sol1 + pz;
                    /* and-table check */
                    for (j = top; j <= bottom; j++)
                    {
                        if (j == pno) continue;
                        if (iof(j, bx, by, bz) > 0)
                        {   /* OUTSIDE */
                            break;
                        }
                    }
                    if (j > bottom)
                    {
                        if (!transchk) break;
                        /* transparent primitive check */
                        for (j = 0; j < use_trans; j++)
                        {
                            if (j == transno) continue;
                            end = atable[trans[j], 1];
                            for (k = atable[trans[j], 0]; k <= end; k++)
                            {
                                if (iof(k, bx, by, bz) > 0) break;
                            }
                            if (k > end) break;
                        }
                        if (j >= use_trans)
                        {
                            break;      /* pass the check */
                        }
                    }
                }
                if (solno > 0)
                {
                    solflag = true;
                    maxt = t = sol1;
                    primno = pno;
                    bbx = bx;
                    bby = by;
                    bbz = bz;
                    if (check)
                    {
                        statt[ano] = sol1;
                        statprimno[ano] = pno;
                        statcross[ano, 0] = (bbx);
                        statcross[ano, 1] = (bby);
                        statcross[ano, 2] = (bbz);
                    }
                }
            }
            return (solflag);
        }

        public static void normal_cross_point(CROSS_POINT pcp, CROSS_POINT acp)
        {
            uint i;
            double st, t = 0.0;
            uint sprimno = 0;
            uint primno = 0;
            double bbx = 0.0, bby = 0.0, bbz = 0.0;
            double sbbx = 0.0, sbby = 0.0, sbbz = 0.0;

            /* clear 'appear_meta_list' */
            appear_meta_list = 0;

            /* copy variables */
            vx = pcp.vx;
            vy = pcp.vy;
            vz = pcp.vz;
            px = pcp.x;
            py = pcp.y;
            pz = pcp.z;
            primchk = pcp.flag;
            count = PRIM(pcp.primno).count;
            pdno = PRIM(pcp.primno).pdno;
            t_scm = pcp.t_scm;
            cp = acp;

            st = maxt = 2 * INFINITY;
            for (i = 0; i < use_atable; i++)
            {
                if (cross_point(i, ref t, ref primno, false, ref bbx, ref bby, ref bbz))
                {
                    st = t;
                    sprimno = primno;
                    sbbx = bbx;
                    sbby = bby;
                    sbbz = bbz;
                }
            }
            if (st >= INFINITY)
            {
                cp.flag = false;
                meta_cross_point();
                return;
            }
            cp.flag = true;
            cp.t = st;
            cp.primno = sprimno;
            cp.x = sbbx;
            cp.y = sbby;
            cp.z = sbbz;
            meta_cross_point();
            return;
        }
    }
}
