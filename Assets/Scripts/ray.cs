using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.IO;
using System;
using UnityEngine;
using static EXRAY.rayh;
using static EXRAY.raysh;
using static EXRAY.ray1;
using static EXRAY.ray2;
using static EXRAY.ray3;
using static EXRAY.ray4;

namespace EXRAY
{
    public class ray
    {
        static public int main(int ScreenX, int ScreenY, int argc, string[] argv)
        {
            //double (*tbl0)[3], (*tbl1)[3];
            double[,] tbl0 = new double[0, 3];
            double[,] tbl1 = new double[0, 3];
            int pxlx, pxly;
            int w;
            double[] mat;
            int x, y;
            FileStream fp;
            double vx, vy, vz;
            string temp_file = "";
            byte[] l_buffer = new byte[0], l_buffer_sub;
            int buffer = 0, buffer_sub;
            int px, py;
            bool cnt_flag;
            int begin_y, begin_yy;
            uint i, j;
            uint k;
            CROSS_POINT cp = new CROSS_POINT();
            FileStream fps;
            //double (*tbl2)[3], (*swap_tbl)[3];
            double[,] tbl2 = new double[0, 3];
            double[,] swap_tbl = new double[0, 3];
            double vl;
            double anti_aliasing_factor = 0.0f;
            double max_diff, s;
            double ss;
            double[] clr = new double[3];
            double max, a, r;

            //EXRAYcontext = currentContext;
            dataPath = argv[0];

            /* check of C-Compiler */
            //a = -2;
            //r = -17.0;
            //if (!((a == fabs(a)) == FALSE && (r != fabs(r)) == TRUE))
            //{
            /* ERROR OF C-COMPILER !!!! */
            //    fprintf(stderr, "\n");
            //    fprintf(stderr, "**************************************************************\n");
            //    fprintf(stderr, "****  This program is dependent for two facts             ****\n");
            //    fprintf(stderr, "****     that are '(a == a) == %1d'                         ****\n", TRUE);
            //    fprintf(stderr, "****     and '(a != a) == %1d' of this use C-Compiler.      ****\n", FALSE);
            //    fprintf(stderr, "**************************************************************\n");
            //    terminate(1);
            //}

            /* Set signals */
            //set_signal();

            /* Starting message */
            //printf("\n");
            Debug.Log("Extended Ray-Tracing Program (Version 2.1)\n");
            Debug.Log("Copyright by Mitsunori Satomi (C) 1988\n");
            //printf("\n");

            /* call I/O program */
            try
            {
                io_main(ScreenX, ScreenY, argc, argv);
            } catch (Exception e)
            {
                Debug.LogError(e);
            }
            /* check '-ct' flag */
            cnt_flag = false;
            for (x = 1; x < argc; x++)
                if (argv[x] == "-ct")
                {
                    cnt_flag = true;
                    break;
                }
            if (cnt_flag)
            {
                //if ((buffer = malloc(view.pxlx * 3 * sizeof(char))) == NULL)
                //{
                //    fprintf(stderr, "*** No memory for allocation. ***\n");
                //    terminate(1);
                //}
                l_buffer = new byte[view.pxlx * 3]; buffer = 0;
                
                //sprintf(temp_file, "temp%d.tmp", getpid());
                temp_file = dataPath + "/temp" + Guid.NewGuid().ToString("N") + ".tmp";

                /* Warning : Arguments of rename function are different          *
                 *           between MS-C(older than version4.0) and             *
                 *           another C-Compiler(UNIX-C or MS-C(version4.0) usw). */
                //rename(view.Object, temp_file);
                File.Move(view.Object, temp_file);
            }
            // Override Screen size.
            //view.pxlx = ScreenX;
            //view.pxly = ScreenY;

            /* object file open */
            //if ((fp = fopen(view.object, WRITE_BINARY)) == NULL)
            //{
            //    fprintf(stderr, "*** Can't open '%s' file. ***\n", view.object);
            //    terminate(1);
            //}
            using (fp = new FileStream(view.Object, FileMode.Create, FileAccess.Write))
            {

                pxlx = view.pxlx;
                pxly = view.pxly;

                //fputc((pxlx / 256), fp);
                //fputc((pxlx % 256), fp);
                //fputc((pxly / 256), fp);
                //fputc((pxly % 256), fp);
                fp.WriteByte((byte)(pxlx / 256));
                fp.WriteByte((byte)(pxlx % 256));
                fp.WriteByte((byte)(pxly / 256));
                fp.WriteByte((byte)(pxly % 256));

                /* read last picture file */
                begin_y = 0;
                if (cnt_flag)
                    //if ((fps = fopen(temp_file, READ_BINARY)) != NULL)
                    using (fps = new FileStream(temp_file, FileMode.Open, FileAccess.Read)) 
                    {
                        //fseek(fps, (long)4, 0);             /* Skip 4 bytes */
                        fps.Seek((long)4, SeekOrigin.Begin);
                        //while (!feof(fps))
                        while (fps.Position < fps.Length) {
                            buffer_sub = buffer;
                            l_buffer_sub = l_buffer;
                            for (x = 0; x < view.pxlx; x++)
                            {
                                //*buffer_sub++ = getc(fps);
                                l_buffer_sub[buffer_sub++] = (byte)fps.ReadByte();
                                //*buffer_sub++ = getc(fps);
                                l_buffer_sub[buffer_sub++] = (byte)fps.ReadByte();
                                //if (feof(fps)) break;
                                if (fps.Position >= fps.Length) break;
                                //*buffer_sub++ = getc(fps);
                                l_buffer_sub[buffer_sub++] = (byte)fps.ReadByte();
                            }
                            if (x < view.pxlx) break;

                            buffer_sub = buffer;
                            for (x = 0; x < view.pxlx; x++)
                            {
                                //fputc(*buffer_sub++, fp);
                                fp.WriteByte(l_buffer_sub[buffer_sub++]);
                                //fputc(*buffer_sub++, fp);
                                fp.WriteByte(l_buffer_sub[buffer_sub++]);
                                //fputc(*buffer_sub++, fp);
                                fp.WriteByte(l_buffer_sub[buffer_sub++]);
                            }
                            begin_y++;
                        }
                        //fclose(fps);
                        //unlink(temp_file);
                    }

                /* initailze cross_point() */
                initialize_cross_point();

                cp.x = view.cx;
                cp.y = view.cy;
                cp.z = view.cz;

                /* view-point In/Out check */
                cp.t_scm = max_scm;
                max = 0;
                for (i = 0; i < use_atable; i++)
                {
                    k = atable[i, 0];
                    if (PRIM(k).kind == 5)
                    {       /* Meta-Ball */
                        vx = cp.x - PRIM(k).cx;
                        vy = cp.y - PRIM(k).cy;
                        vz = cp.z - PRIM(k).cz;
                        r = (PRIM(k).par[0] * vx + 2 * (PRIM(k).par[4] * vz + PRIM(k).par[5] * vy)) * vx
                        + (PRIM(k).par[1] * vy + 2 * PRIM(k).par[3] * vz) * vy
                            + PRIM(k).par[2] * vz * vz;
                        if (r >= (-PRIM(k).par[6]))
                        {   /* r >= b(No effect) */
                            continue;
                        }
                        r = Math.Sqrt(r);
                        /* compute f(r) */
                        if (r < PRIM(k).par[8])
                        {   /* r < b/3 */
                            a = r * PRIM(k).par[9];
                            a = PRIM(k).par[12] * (1.0 - 3 * a * a);
                        }
                        else
                        {               /* b/3 <= r < b */
                            a = 1.0 - r * PRIM(k).par[9];
                            a *= a * PRIM(k).par[13];
                        }
                        if (a > max)
                        {
                            max = a;
                            cp.primno = k;
                            cp.t_scm = PRIM(k).scm;
                        }
                    }
                    else
                    {               /* No Meta-Ball */
                        k = atable[i, 1];
                        for (j = atable[i, 0]; j <= k; j++)
                        {
                            if (iof(j, cp.x, cp.y, cp.z) > 0)
                            {
                                break;
                            }
                        }
                        if (j > k)
                        {
                            cp.primno = atable[i, 0];
                            cp.t_scm = PRIM(cp.primno).scm;
                        }
                    }
                    if (cp.t_scm < use_scm && scm[cp.t_scm].type != 2)
                    {
                        Debug.LogError("*** The view-point is in the opaque primitives. ***\n");
                        //terminate(1);
                        throw new ApplicationException();
                    }
                }
                if (cp.t_scm < use_scm)
                {
                    scmcpy((cp.t_scms), scm[cp.t_scm]);
                    Debug.Log("Warning : The view-point is in the transparent primitives.\n");
                }

                if (0 != view.anti)
                {
                    /* Anti-aliasing */
                    anti_aliasing_factor = view.anti_aliasing_factor;
                    anti_aliasing_factor *= anti_aliasing_factor;
                    //if ((tbl0 = (double(*)[3])malloc(pxlx * 3 * sizeof(double))) == NULL
                    //   || (tbl1 = (double(*)[3])malloc(pxlx * 3 * sizeof(double))) == NULL
                    //  || (tbl2 = (double(*)[3])malloc(pxlx * 3 * sizeof(double))) == NULL) {
                    //    fprintf(stderr, "*** No memory for allocation. ***\n");
                    //    terminate(1);
                    //}
                    tbl0 = new double[pxlx * 3, 3];
                    tbl1 = new double[pxlx * 3, 3];
                    tbl2 = new double[pxlx * 3, 3];
                }
                mat = view.mat;

                /* Starting Ray-Tracing */
                if ((begin_yy = begin_y - 2) < 0)
                {
                    begin_yy = 0;
                }

                for (y = begin_yy; y <= pxly; y++)
                {
                    Debug.Log(y);
                    //fflush(fp);     /* flush the fp */
                    fp.Flush();

                    if (y != pxly)
                    {

                        Debug.Log(String.Format("Scanning {0:N} lines(all {1:N} lines).\n", y, pxly));

                        for (x = 0; x < pxlx; x++)
                        {

                            vx = view.k1 * (x + view.k2);
                            vy = view.k3 * (y + view.k4);
                            vz = view.scrl;

                            /* Rotation */
                            cp.vx = vx * mat[0] + vy * mat[1] + vz * mat[2];
                            cp.vy = vx * mat[3] + vy * mat[4] + vz * mat[5];
                            cp.vz = vx * mat[6] + vy * mat[7] + vz * mat[8];
                            vl = 1.0 / Math.Sqrt(cp.vx * cp.vx + cp.vy * cp.vy + cp.vz * cp.vz);
                            cp.vx *= vl;
                            cp.vy *= vl;
                            cp.vz *= vl;

                            /* Ray-Tracing */
                            try
                            {
                                ray_trace(cp, (int)(-1), (double)(1.0));
                            } catch (Exception e)
                            {
                                Debug.LogError(e);
                            }

                            if (0 != view.anti)
                            {
                                /* Anti-aliasing */
                                tbl2[x, 0] = cp.r;
                                tbl2[x, 1] = cp.g;
                                tbl2[x, 2] = cp.b;
                            }
                            else
                            {
                                Color32 c = new Color32();
                                if ((w = (int)Math.Floor(cp.r * 255 + .5)) > 255)
                                {
                                    w = 255;
                                }
                                //fputc(w, fp);
                                fp.WriteByte((byte)w);
                                c.r = (byte)w;
                                if ((w = (int)Math.Floor(cp.g * 255 + .5)) > 255)
                                {
                                    w = 255;
                                }
                                //fputc(w, fp);
                                fp.WriteByte((byte)w);
                                c.g = (byte)w;
                                if ((w = (int)Math.Floor(cp.b * 255 + .5)) > 255)
                                {
                                    w = 255;
                                }
                                //fputc(w, fp);
                                fp.WriteByte((byte)w);
                                c.b = (byte)w;
                                c.a = 0xff;
                                int cx = x; int cy = y;
                                //EXRAYcontext.Post(_ =>
                                //{
                                //    EXRAYtracer.SetPixel(cx, 511 - cy, c);
                                //}, null);
                                EXRAYtracer.queue.Enqueue(new EXRAYtracer.OnePixel(x, y, c));
                            }
                        }
                    }

                    if (0 != view.anti)
                    {
                        if (y == 1)
                        {
                            for (x = 0; x < pxlx; x++)
                            {
                                tbl0[x, 0] = tbl1[x, 0];
                                tbl0[x, 1] = tbl1[x, 1];
                                tbl0[x, 2] = tbl1[x, 2];
                            }
                        }
                        if (y == pxly)
                        {
                            for (x = 0; x < pxlx; x++)
                            {
                                tbl2[x, 0] = tbl1[x, 0];
                                tbl2[x, 1] = tbl1[x, 1];
                                tbl2[x, 2] = tbl1[x, 2];
                            }
                        }
                        if (y > begin_y)
                        {
                            for (x = 0; x < pxlx; x++)
                            {
                                max_diff = 0;
                                for (px = x - 1; px <= x + 1; px++)
                                {
                                    if (px < 0 || px >= pxlx)
                                    {
                                        continue;
                                    }

                                    s = tbl0[px, 0] - tbl1[x, 0];
                                    s *= s;
                                    ss = tbl0[px, 1] - tbl1[x, 1];
                                    s += ss * ss;
                                    ss = tbl0[px, 2] - tbl1[x, 2];
                                    if ((s += ss * ss) > max_diff)
                                    {
                                        max_diff = s;
                                    }

                                    s = tbl1[px, 0] - tbl1[x, 0];
                                    s *= s;
                                    ss = tbl1[px, 1] - tbl1[x, 1];
                                    s += ss * ss;
                                    ss = tbl1[px, 2] - tbl1[x, 2];
                                    if ((s += ss * ss) > max_diff)
                                    {
                                        max_diff = s;
                                    }

                                    s = tbl2[px, 0] - tbl1[x, 0];
                                    s *= s;
                                    ss = tbl2[px, 1] - tbl1[x, 1];
                                    s += ss * ss;
                                    ss = tbl2[px, 2] - tbl1[x, 2];
                                    if ((s += ss * ss) > max_diff)
                                    {
                                        max_diff = s;
                                    }
                                }
                                if (max_diff < anti_aliasing_factor)
                                {
                                    /* No,anti-aliasing */
                                    for (i = 0; i < 3; i++)
                                    {
                                        clr[i] = tbl1[x, i];
                                    }
                                }
                                else
                                {       /* Anti-aliasing */
                                    s = 1.0 / ((double)view.anti + 1.0);

                                    for (px = 0; px <= view.anti; px++)
                                    {
                                        for (py = 0; py <= view.anti; py++)
                                        {
                                            if (px == 0 && py == 0)
                                            {
                                                for (i = 0; i < 3; i++)
                                                {
                                                    clr[i] = tbl1[x, i];
                                                }
                                            }
                                            else
                                            {
                                                vx = view.k1 * ((x + px * s) + view.k2);
                                                vy = view.k3 * ((y - 1.0 + py * s) + view.k4);
                                                vz = view.scrl;

                                                /* Rotation */
                                                cp.vx = vx * mat[0] + vy * mat[1] + vz * mat[2];
                                                cp.vy = vx * mat[3] + vy * mat[4] + vz * mat[5];
                                                cp.vz = vx * mat[6] + vy * mat[7] + vz * mat[8];
                                                vl = 1.0 / Math.Sqrt(cp.vx * cp.vx + cp.vy * cp.vy + cp.vz * cp.vz);
                                                cp.vx *= vl;
                                                cp.vy *= vl;
                                                cp.vz *= vl;

                                                /* Ray-Tracing */
                                                ray_trace(cp, (int)(-1), (double)(1.0));

                                                clr[0] += cp.r;
                                                clr[1] += cp.g;
                                                clr[2] += cp.b;
                                            }
                                        }
                                    }

                                    /* Meaning */
                                    s *= s;
                                    for (i = 0; i < 3; i++)
                                    {
                                        clr[i] *= s;
                                    }
                                }

                                /* Output to picture file */
                                Color32 c = new Color32();
                                c.a = 0xff;
                                for (i = 0; i < 3; i++)
                                {
                                    if ((w = (int)Math.Floor(clr[i] * 255 + 0.5)) > 255)
                                    {
                                        w = 255;
                                    }
                                    //fputc(w, fp);
                                    switch (i)
                                    {
                                        case 0: c.r = (byte)w; break;
                                        case 1: c.g = (byte)w; break;
                                        case 2: c.b = (byte)w; break;
                                    }
                                    fp.WriteByte((byte)w);
                                    EXRAYtracer.queue.Enqueue(new EXRAYtracer.OnePixel(x, y, c));
                                }
                            }
                        }

                        /* Rotate tbl0-3 */
                        swap_tbl = tbl0;
                        tbl0 = tbl1;
                        tbl1 = tbl2;
                        tbl2 = swap_tbl;
                    }
                }

                /* close object file */
                //fclose(fp);
            }

            /* terminated message */
            Debug.Log("Terminated Extended Ray-Tracing Program.\n");

            /* normal terminated */
            //terminate(0);
            return 0;
        }
    }
}