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
//using static EXRAY.ray3;

namespace EXRAY
{
    public partial class ray34
    {
        public void normal_vector(CROSS_POINT cp)
        {
            double[] p;
            uint i;
            uint j = 0;
            tag_prim tprim;
            double bx, by, bz;
            double nx, ny, nz;
            double nl;
            double k;
            double min;

            tprim = PRIM(cp.primno);
            p = tprim.par;
            if (tprim.kind == 2)
            {   /* HEIMEN */
                cp.nx = p[0];
                cp.ny = p[1];
                cp.nz = p[2];
                return;
            }
            bx = cp.x - tprim.cx;
            by = cp.y - tprim.cy;
            bz = cp.z - tprim.cz;
            switch (tprim.kind)
            {
                case 1:
                    min = INFINITY;
                    for (i = 0; i < 12; i += 4)
                    {
                        if ((k = Math.Abs(Math.Abs(p[i] * bx + p[i + 1] * by + p[i + 2] * bz) + p[i + 3])) < min)
                        {
                            j = i;
                            min = k;
                        }
                    }
                    cp.nx = p[j];
                    cp.ny = p[j + 1];
                    cp.nz = p[j + 2];
                    break;

                case 3:         /* 2JI KYOKUMEN */
                case 4:
                    nx = p[0] * bx + p[4] * bz + p[5] * by;
                    ny = p[1] * by + p[3] * bz + p[5] * bx;
                    nz = p[2] * bz + p[3] * by + p[4] * bx;
                    if ((nl = Math.Sqrt(nx * nx + ny * ny + nz * nz)) == 0)
                    {
                        nx = 1;
                    }
                    else
                    {
                        nl = 1.0 / nl;  /* Normalize */
                        nx *= nl;
                        ny *= nl;
                        nz *= nl;
                    }
                    cp.nx = nx;
                    cp.ny = ny;
                    cp.nz = nz;
                    break;

                default:
                    Debug.LogError("*** Inside error(normal_vector()). ***\n");
                    terminate(1);
                    break;
            }
        }

        public void sub_mapping(CROSS_POINT cp)
        {
            double[] p;
            tag_prim tprim;
            uint scmno;
            tag_scm tscm;
            uint scm1, scm2;
            double bx, by, bz;
            double px, py, pz;
            double theta, alpha;
            int c;
            long ax, ay, pt;
            double u = 0.0, w = 0.0;
            double phi;
            double sx, sy;
            double r;

            tprim = PRIM(cp.primno);
            scmno = tprim.scm;
            if (scm[scmno].source_no == 0)
            {
                //cp.scm = scmno;
                if (cp.scms == null) cp.scms = new tag_scm();
                scmcpy(cp.scms, scm[scmno]);
                return;
            }
            /* convert global coordinates to local ones */
            bx = cp.x - tprim.cx;
            by = cp.y - tprim.cy;
            bz = cp.z - tprim.cz;
            p = tprim.mat;
            px = bx * p[0] + by * p[1] + bz * p[2];
            py = bx * p[3] + by * p[4] + bz * p[5];
            pz = bx * p[6] + by * p[7] + bz * p[8];

            /* convert primitive coordinates to mapping ones */
            tscm = scm[scmno];
            px -= tscm.mapping.ox;
            py -= tscm.mapping.oy;
            pz -= tscm.mapping.oz;
            p = tscm.mapping.gmat;
            bx = (p[0] * px + p[1] * py + p[2] * pz) * tscm.mapping.sclx;
            by = (p[3] * px + p[4] * py + p[5] * pz) * tscm.mapping.scly;
            bz = (p[6] * px + p[7] * py + p[8] * pz) * tscm.mapping.sclz;

            /* convert mapping coordinates to (u-w) ones */
            switch (tscm.mapping.mapping_no)
            {
                default:
                    Debug.LogError("*** Inside error(sub_mapping()). ***\n");
                    terminate(1);
                    break;

                case 0:         /* plane mapping */
                    u = bx - Math.Floor(bx);
                    w = by - Math.Floor(by);
                    break;

                case 1:
                    if ((theta = bx * bx + by * by) != 0)
                    {
                        theta = Math.Acos(bx / Math.Sqrt(theta));
                        if (by < 0) theta = (2 * PI) - theta;
                    }
                    u = theta * (1 / (2 * PI));
                    w = bz - Math.Floor(bz);
                    break;

                case 2:
                    if ((theta = bx * bx + by * by) == 0)
                    {
                        phi = 0;
                    }
                    else
                    {
                        phi = Math.Asin(bz / Math.Sqrt(theta + bz * bz));
                        theta = Math.Acos(bx / Math.Sqrt(theta));
                        if (by < 0) theta = (2 * PI) - theta;
                    }
                    u = theta * (1 / (2 * PI));
                    w = phi * (1 / PI) + 0.5;
                    break;
            }

            //cp.scm = scmno;
            if (cp.scms == null) cp.scms = new tag_scm();
            scmcpy(cp.scms, scm[scmno]);
            if (tscm.source_no < 3)
            {/* file mapping */
                ax = (long)(u * tscm.mapping.fx);
                ay = (long)((1.0 - w) * tscm.mapping.fy);
                if (ax < 0)
                {
                    ax = 0;
                }
                else
                {
                    if (ax >= tscm.mapping.fx)
                    {
                        ax = tscm.mapping.fx - 1;
                    }
                }
                if (ay < 0)
                {
                    ay = 0;
                }
                else
                {
                    if (ay >= tscm.mapping.fy)
                    {
                        ay = tscm.mapping.fy - 1;
                    }
                }
                pt = (ax + ay * ((long)tscm.mapping.fx)) * 3;
                if (tscm.source_no == 1)
                {
                    if ((c = tscm.mapping.mem[(uint)pt]) < 0)
                    {
                        c += 256;
                    }
                    cp.scms.r = ((double)c) * (1 / 255.0);
                    if ((c = tscm.mapping.mem[(uint)(pt + 1)]) < 0)
                    {
                        c += 256;
                    }
                    cp.scms.g = ((double)c) * (1 / 255.0);
                    if ((c = tscm.mapping.mem[(uint)(pt + 2)]) < 0)
                    {
                        c += 256;
                    }
                    cp.scms.b = ((double)c) * (1 / 255.0);
                }
                else
                {
                    //fseek(tscm.mapping.fp, (long)(pt + 4), 0);
                    tscm.mapping.fp.Seek(pt + 4, SeekOrigin.Begin);
                    //scm[scmno].r = ((double)fgetc(tscm.mapping.fp)) * (1 / 255.0);
                    //scm[scmno].g = ((double)fgetc(tscm.mapping.fp)) * (1 / 255.0);
                    //scm[scmno].b = ((double)fgetc(tscm.mapping.fp)) * (1 / 255.0);
                    cp.scms.r = ((double)tscm.mapping.fp.ReadByte() * (1 / 255.0));
                    cp.scms.g = ((double)tscm.mapping.fp.ReadByte() * (1 / 255.0));
                    cp.scms.b = ((double)tscm.mapping.fp.ReadByte() * (1 / 255.0));
                }
            }
            else
            {
                sx = tscm.mapping.x + u * tscm.mapping.dx;
                sy = tscm.mapping.y + w * tscm.mapping.dy;
                switch (tscm.source_no)
                {
                    case 3:
                        //if (!(NEGATIVE(sx - Math.Floor(sx) - 0.5) ^ NEGATIVE(sy - Math.Floor(sy) - 0.5) > 0))
                        if (!(NEGATIVE(sx - Math.Floor(sx) - 0.5) ^ NEGATIVE(sy - Math.Floor(sy) - 0.5)))
                        {
                            //cp.scm = tscm.mapping.scm[0];
                            scmcpy(cp.scms, scm[tscm.mapping.scm[0]]);
                        }
                        else
                        {
                            //cp.scm = tscm.mapping.scm[1];
                            scmcpy(cp.scms, scm[tscm.mapping.scm[1]]);
                        }
                        break;

                    case 4:
                        if (sy - Math.Floor(sy) < 0.5)
                        {
                            //cp.scm = tscm.mapping.scm[0];
                            scmcpy(cp.scms, scm[tscm.mapping.scm[0]]);
                        }
                        else
                        {
                            //cp.scm = tscm.mapping.scm[1];
                            scmcpy(cp.scms, scm[tscm.mapping.scm[1]]);
                        }
                        break;

                    case 5:
                        alpha = Math.Sin(PI * (sy - 0.25));
                        alpha *= alpha;
                        scm1 = tscm.mapping.scm[0];
                        scm2 = tscm.mapping.scm[1];
                        cp.scms.r = (1 - alpha) * scm[scm1].r + alpha * scm[scm2].r;
                        cp.scms.g = (1 - alpha) * scm[scm1].g + alpha * scm[scm2].g;
                        cp.scms.b = (1 - alpha) * scm[scm1].b + alpha * scm[scm2].b;
                        break;

                    case 6:
                        if (sx - Math.Floor(sx) < 0.5)
                        {
                            //cp.scm = tscm.mapping.scm[0];
                            scmcpy(cp.scms, scm[tscm.mapping.scm[0]]);
                        }
                        else
                        {
                            //cp.scm = tscm.mapping.scm[1];
                            scmcpy(cp.scms, scm[tscm.mapping.scm[1]]);
                        }
                        break;

                    case 7:
                        alpha = Math.Sin(PI * (sx - 0.25));
                        alpha *= alpha;
                        scm1 = tscm.mapping.scm[0];
                        scm2 = tscm.mapping.scm[1];
                        cp.scms.r = (1 - alpha) * scm[scm1].r + alpha * scm[scm2].r;
                        cp.scms.g = (1 - alpha) * scm[scm1].g + alpha * scm[scm2].g;
                        cp.scms.b = (1 - alpha) * scm[scm1].b + alpha * scm[scm2].b;
                        break;

                    case 8:
                        r = Math.Sqrt(sx * sx + sy * sy) + 0.25;
                        if (r - Math.Floor(r) < 0.5)
                        {
                            //cp.scm = tscm.mapping.scm[0];
                            scmcpy(cp.scms, scm[tscm.mapping.scm[0]]);
                        }
                        else
                        {
                            //cp.scm = tscm.mapping.scm[1];
                            scmcpy(cp.scms, scm[tscm.mapping.scm[1]]);
                        }
                        break;

                    case 9:
                        r = Math.Sqrt(sx * sx + sy * sy);
                        alpha = Math.Sin(PI * r);
                        alpha *= alpha;
                        scm1 = tscm.mapping.scm[0];
                        scm2 = tscm.mapping.scm[1];
                        cp.scms.r = (1 - alpha) * scm[scm1].r + alpha * scm[scm2].r;
                        cp.scms.g = (1 - alpha) * scm[scm1].g + alpha * scm[scm2].g;
                        cp.scms.b = (1 - alpha) * scm[scm1].b + alpha * scm[scm2].b;
                        break;
                }
            }
        }

        public void mapping(CROSS_POINT cp)
        {
            uint i;
            uint primno;
            uint j;
            double[] p;
            //uint scmno;
            uint[] l_sub_meta_list = meta_list;
            int sub_meta_list = 0;
            double[][] l_sub_meta_list_par = meta_list_par;
            tag_prim tprim;
            double lx, ly, lz, a;
            //uint h_scmno;
            bool h_scmflag = false;
            double nx, ny, nz, nl;
            double cr, cg, cb;
            double fd, fh;
            double b;
            double r;

            if (PRIM(cp.primno).kind != 5)
            {   /* No Meta-Ball */
                normal_vector(cp);
                sub_mapping(cp);
                //scmcpy((cp.scms), scm[cp.scm]);
                return;
            }
            /* Meta-Ball */
            nx = ny = nz = 0;
            cr = cg = cb = 0;
            fd = fh = 0;
            primno = cp.primno;
            //h_scmno = max_scm;
            tag_scm h_scm = new tag_scm();

            for (i = use_meta_list; i > 0; i--, sub_meta_list++)
            {
                //p = (tprim = PRIM(cp.primno = j = l_sub_meta_list[sub_meta_list++])).par;
                tprim = PRIM(cp.primno = j = l_sub_meta_list[sub_meta_list]);
                p = l_sub_meta_list_par[sub_meta_list];
                lx = cp.x - tprim.cx;
                ly = cp.y - tprim.cy;
                lz = cp.z - tprim.cz;
                r = (p[0] * lx + 2 * (p[4] * lz + p[5] * ly)) * lx
                    + (p[1] * ly + 2 * p[3] * lz) * ly + p[2] * lz * lz;
                if (r >= p[14]) continue;
                r = Math.Sqrt(r);
                if (r < p[8])
                {
                    a = r * p[9];
                    a = p[12] * (1.0 - 3 * a * a);
                    b = p[10];
                }
                else
                {
                    a = 1.0 - r * p[9];
                    a *= a * p[13];
                    b = p[11] * (1.0 / r - p[9]);
                }
                /* Normal Vector */
                nx += b * (p[0] * lx + p[4] * lz + p[5] * ly);
                ny += b * (p[1] * ly + p[3] * lz + p[5] * lx);
                nz += b * (p[2] * lz + p[3] * ly + p[4] * lx);

                /* Mapping */
                sub_mapping(cp);
                //scmno = cp.scm;
                if (primno == j)
                {   /* Most effective SCM */
                    h_scmflag = true;
                    //h_scmno = scmno;
                    scmcpy(h_scm, cp.scms);
                    //scmcpy((cp.scms), scm[scmno]);
                }
                /* a is weight */
                cr += a * cp.scms.r;
                cg += a * cp.scms.g;
                cb += a * cp.scms.b;
                fd += a * cp.scms.fd;
                fh += a * cp.scms.fh;
            }
            /* Normalization */
            if ((nl = Math.Sqrt(nx * nx + ny * ny + nz * nz)) == 0)
            {
                cp.nx = 1;
                cp.ny = cp.nz = 0;
            }
            else
            {
                nl = 1.0 / nl;
                cp.nx = nx * nl;
                cp.ny = ny * nl;
                cp.nz = nz * nl;
            }
            /* Range Check */
            if (cr < 0) cr = 0;
            else if (cr > 1) cr = 1;
            if (cg < 0) cg = 0;
            else if (cg > 1) cg = 1;
            if (cb < 0) cb = 0;
            else if (cb > 1) cb = 1;
            if (fd < 0) fd = 0;
            else if (fd > 1) fd = 1;
            if (fh < 0) fh = 0;
            else if (fh > 1) fh = 1;
            cp.scms = h_scm;
            cp.scms.r = cr;
            cp.scms.g = cg;
            cp.scms.b = cb;
            cp.scms.fd = fd;
            cp.scms.fh = fh;
            cp.primno = primno;
            //if (h_scmno >= use_scm)
            if (!h_scmflag)
            {
                Debug.LogError("*** Inside error(mapping()). ***\n");
                terminate(1);
            }
        }

        public void shadowing(uint light_no, CROSS_POINT cp, double vl)
        {
            double r, g, b;
            double base_t;
            double vls;
            CROSS_POINT cps = new CROSS_POINT();

            /* shadowing */
            r = light[light_no].r;
            g = light[light_no].g;
            b = light[light_no].b;

            tag_view view = views[0];
            if (!view.shadow)
            {       /* No shadowing */
                cp.r = r;
                cp.g = g;
                cp.b = b;
                return;
            }
            cps.x = cp.x;
            cps.y = cp.y;
            cps.z = cp.z;
            cps.vx = cp.vx;
            cps.vy = cp.vy;
            cps.vz = cp.vz;
            cps.primno = cp.primno;
            cps.t_scms = cp.t_scms;
            //scmcpy(&cps.t_scms, &cp->t_scms);
            cps.flag = cp.flag;
            base_t = 0;

            while (true)
            {
                if (view.method)
                {
                    btree_cross_point(cps, cps);
                }
                else
                {
                    normal_cross_point(cps, cps);
                }
                if (!cps.flag || cps.t >= vl)
                {   /* No solution */
                    base_t += vl;
                    //if (cps.t_scm < use_scm)
                    if (null != cps.t_scms)
                    {
                        r *= Math.Exp(cps.t_scms.ka * base_t);
                        g *= Math.Exp(cps.t_scms.kb * base_t);
                        b *= Math.Exp(cps.t_scms.kc * base_t);
                    }
                    break;
                }
                base_t += cps.t;
                mapping(cps);
                /* Opaque or Mirror primitive */
                if (cps.scms.type < 2)
                {
                    r = g = b = 0;
                    break;
                }
                /* transparent primitive */
                if (cps.scms.type == 2)
                {
                    vls = cps.scms.fs2 * (1 - cal_value(Math.Abs(cps.nx * cps.vx + cps.ny * cps.vy + cps.nz * cps.vz)
                    //                , (cps.t_scm >= use_scm ? cps.scms.rf[0] : cps.scms.rf[1])));
                                    , (null == cps.t_scms ? cps.scms.rf[0] : cps.scms.rf[1])));
                    vls = 1.0 + view.ts_factor * (vls - 1.0);
                    r *= vls;
                    g *= vls;
                    b *= vls;
                    //if (cps.t_scm >= use_scm)
                    if (null == cps.t_scms)
                    {
                        //cps.t_scm = cps.scm;
                        cps.t_scms = new tag_scm();
                        scmcpy(cps.t_scms, cps.scms);
                        base_t = 0;
                    }
                    else
                    {
                        //cps.t_scm = max_scm;
                        r *= Math.Exp(cps.t_scms.ka * base_t);
                        g *= Math.Exp(cps.t_scms.kb * base_t);
                        b *= Math.Exp(cps.t_scms.kc * base_t);
                        cps.t_scms = null;
                    }
                }
                vl -= cps.t;
            }
            cp.r = r;
            cp.g = g;
            cp.b = b;
        }

        public void factor(double h_s, double h_n, double l_n, double n_s
        , CROSS_POINT cp, double[] fac)
        {
            double a;

            /* Fac=F*D*G/(N*S) */

            /* D */
            fac[0] = cal_value(h_n, cp.scms.d);

            /* G */
            if (n_s >= l_n)
            {
                if ((a = 2 * h_n * l_n / h_s) < 1)
                {
                    fac[0] *= a;
                }
            }
            else
            {
                if ((a = 2 * h_n * n_s / h_s) < 1)
                {
                    fac[0] *= a;
                }
            }

            fac[0] /= n_s;

            /* F(separate in colors) */
            fac[2] = fac[0] * cal_value(h_s, cp.scms.f[2]);
            fac[1] = fac[0] * cal_value(h_s, cp.scms.f[1]);
            fac[0] *= cal_value(h_s, cp.scms.f[0]);
        }

        private const double SQRT3 = (1.0 / 1.73205080756);
        public void ray_trace(CROSS_POINT cp, int level, double power)
        {
            int light_no;
            bool status;
            CROSS_POINT cps = new CROSS_POINT(); ;
            double base_t;
            double sx, sy, sz;
            double r, g, b;
            double vl, vls;
            double l_n;
            double rx, ry, rz;
            double hx, hy, hz, hl;
            double[] fac = new double[3];
            double max_fac;
            double rf;
            double rrx, rry, rrz, rrl;
            double n_s;

            base_t = 0;
            level++;
            cp.r = cp.g = cp.b = 0;

            tag_view view = views[0];
            /* Terminal condition */
            if (level > view.depth_level || power < view.limit_power)
            {
                return;
            }
            cps.x = cp.x;
            cps.y = cp.y;
            cps.z = cp.z;
            cps.vx = cp.vx;
            cps.vy = cp.vy;
            cps.vz = cp.vz;
            cps.flag = cp.flag;
            cps.primno = cp.primno;
            cps.t_scms = cp.t_scms;
            //scmcpy(cps.t_scms, cp.t_scms);

            /* Ray-Tracing */
            if (view.method)
            {
                btree_cross_point(cps, cps);
            }
            else
            {
                normal_cross_point(cps, cps);
            }
            if (!cps.flag)
            {
                return;         /* No cross point */
            }
            base_t += cps.t;

            //EXRAYcontext.Post(_ =>
            //{
            //    Vector3 p1 = new Vector3((float)cp.x, (float)cp.y, (float)cp.z);
            //    Vector3 p2 = new Vector3((float)cps.x, (float)cps.y, (float)cps.z);
            //    EXRAYtracer.MakeLine(p1, p2);
            //}
            //, null);
            if (false) {
                Vector3 p1 = new Vector3((float)cp.x, (float)cp.y, (float)cp.z);
                Vector3 p2 = new Vector3((float)cps.x, (float)cps.y, (float)cps.z);
                EXRAYtracer.queueLine.Enqueue(new EXRAYtracer.OneLine(p1, p2));
            }
            //Thread.Sleep(1);

            /* Shading */
            mapping(cps);
            sx = -cps.vx;
            sy = -cps.vy;
            sz = -cps.vz;
            if ((n_s = sx * cps.nx + sy * cps.ny + sz * cps.nz) < 0)
            {
                n_s = -n_s;
                cps.nx = -cps.nx;
                cps.ny = -cps.ny;
                cps.nz = -cps.nz;
            }
            rx = 2 * n_s * cps.nx - sx;
            ry = 2 * n_s * cps.ny - sy;
            rz = 2 * n_s * cps.nz - sz;

            for (light_no = 0; light_no < use_light; light_no++)
            {
                /* Ambient */
                r = g = b = light[light_no].light;
                r *= light[light_no].r;
                g *= light[light_no].g;
                b *= light[light_no].b;
                /* Lj */
                cps.vx = light[light_no].cx - cps.x;
                cps.vy = light[light_no].cy - cps.y;
                cps.vz = light[light_no].cz - cps.z;
                if ((vl = Math.Sqrt(cps.vx * cps.vx + cps.vy * cps.vy + cps.vz * cps.vz)) == 0)
                {
                    cps.vx = 1;
                }
                else
                {
                    vls = 1.0 / vl;
                    cps.vx *= vls;
                    cps.vy *= vls;
                    cps.vz *= vls;
                }
                status = false;
                if ((l_n = cps.vx * cps.nx + cps.vy * cps.ny + cps.vz * cps.nz) > 0)
                {
                    shadowing((uint)light_no, cps, vl);
                    if (cps.r != 0 || cps.g != 0 || cps.b != 0)
                    {
                        status = true;
                        /* Diffuse */
                        r += cps.r * l_n;
                        g += cps.g * l_n;
                        b += cps.b * l_n;
                    }
                }
                cp.r += cps.scms.fd * cps.scms.r * r;
                cp.g += cps.scms.fd * cps.scms.g * g;
                cp.b += cps.scms.fd * cps.scms.b * b;

                /* Specular */
                if (status)
                {
                    if (cps.scms.hm == 0)
                    {   /* Phong Model */
                        max_fac = rx * cps.vx + ry * cps.vy + rz * cps.vz;
                        if (max_fac <= 0)
                        {
                            fac[0] = fac[1] = fac[2] = 0;
                        }
                        else
                        {
                            fac[0] = fac[1] = fac[2] = cal_value(max_fac, cps.scms.fac);
                        }
                    }
                    else
                    {           /* Blinn Model */
                        hx = sx + cps.vx;
                        hy = sy + cps.vy;
                        hz = sz + cps.vz;
                        if ((hl = Math.Sqrt(hx * hx + hy * hy + hz * hz)) == 0)
                        {
                            hx = 1;
                        }
                        else
                        {
                            hl = 1.0 / hl;
                            hx *= hl;
                            hy *= hl;
                            hz *= hl;
                        }
                        factor((double)(hx * sx + hy * sy + hz * sz)
                               , (double)(hx * cps.nx + hy * cps.ny + hz * cps.nz)
                               , l_n, n_s, cps, fac);
                    }
                    cp.r += fac[0] * cps.r * cps.scms.fh;
                    cp.g += fac[1] * cps.g * cps.scms.fh;
                    cp.b += fac[2] * cps.b * cps.scms.fh;
                }
            }
            if (cps.scms.type == 1)
            {   /* Mirror */
                if (cps.scms.hm != 0)
                {   /* Blinn Model */
                    factor(n_s, (double)(1.0), n_s, n_s, cps, fac);
                    max_fac = Math.Sqrt(fac[0] * fac[0] + fac[1] * fac[1]
                           + fac[2] * fac[2]) * SQRT3;
                }
                else
                {
                    fac[0] = fac[1] = fac[2] = max_fac = 1.0;
                }
                cps.vx = rx;
                cps.vy = ry;
                cps.vz = rz;
                cps.t_scms = cp.t_scms;
                //scmcpy(cps.t_scms, cp.t_scms);
                ray_trace(cps, level, power * max_fac);
                cp.r += fac[0] * cps.r * cps.scms.fs1;
                cp.g += fac[1] * cps.g * cps.scms.fs1;
                cp.b += fac[2] * cps.b * cps.scms.fs1;
            }
            else if (cps.scms.type == 2)
            {   /* transparent */
                //rf = cal_value(n_s, (cp.t_scm >= use_scm ? cps.scms.rf[0] : cps.scms.rf[1]));
                rf = cal_value(n_s, (null == cp.t_scms ? cps.scms.rf[0] : cps.scms.rf[1]));
                if (level < view.branch_level || rf >= 0.5)
                {
                    /* Reflection */
                    cps.vx = rx;
                    cps.vy = ry;
                    cps.vz = rz;
                    cps.t_scms = cp.t_scms;
                    //scmcpy(cps.t_scms, cp.t_scms);
                    max_fac = rf * cps.scms.fs2;
                    ray_trace(cps, level, power * max_fac);
                    cp.r += max_fac * cps.r;
                    cp.g += max_fac * cps.g;
                    cp.b += max_fac * cps.b;
                }
                if (level < view.branch_level || rf < 0.5)
                {
                    /* Refraction */
                    //if (cp.t_scm >= use_scm)
                    if (null == cp.t_scms)
                    {
                        rrl = cps.scms.n;
                    }
                    else
                    {
                        rrl = 1.0 / cps.scms.n;
                    }
                    if ((rrl = rrl * rrl + n_s * n_s - 1.0) < 0)
                    {
                        goto EXIT;
                    }
                    rrl = Math.Sqrt(rrl) - n_s;
                    rrx = -(sx + rrl * cps.nx);
                    rry = -(sy + rrl * cps.ny);
                    rrz = -(sz + rrl * cps.nz);
                    rrl = 1.0 / Math.Sqrt(rrx * rrx + rry * rry + rrz * rrz);
                    cps.vx = rrx * rrl;
                    cps.vy = rry * rrl;
                    cps.vz = rrz * rrl;
                    //if (cp.t_scm >= use_scm)
                    if (null == cp.t_scms)
                    {
                        //cps.t_scm = cps.scm;
                        cps.t_scms = new tag_scm();
                        scmcpy(cps.t_scms, cps.scms);
                    }
                    else
                    {
                        //cps.t_scm = max_scm;
                        cps.t_scms = null;
                    }
                    max_fac = (1 - rf) * cps.scms.fs2;
                    ray_trace(cps, level, power * max_fac);
                    cp.r += max_fac * cps.r;
                    cp.g += max_fac * cps.g;
                    cp.b += max_fac * cps.b;
                }
            EXIT:;
            }
            //if (cp.t_scm < use_scm)
            if (null != cp.t_scms)
            {
                cp.r *= Math.Exp(cp.t_scms.ka * base_t);
                cp.g *= Math.Exp(cp.t_scms.kb * base_t);
                cp.b *= Math.Exp(cp.t_scms.kc * base_t);
            }
        }
    }
}
