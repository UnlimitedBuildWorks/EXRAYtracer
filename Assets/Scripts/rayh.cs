using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace EXRAY
{
    public class rayh {
        /*
         * Ray-Tracing header file
         * Copyright by Mitsunori Satomi
         * $Author: satomi $ $Revision: 1.12 $ $Date: 88/09/22 18:22:38 $
         * $Header: ray.h,v 1.12 88/09/22 18:22:38 satomi Exp $
         */

        /* definition of logical constants(true/false) */
        //#ifndef TRUE
        //#define TRUE  1
        //#define FALSE 0
        //#endif
        public const int TRUE = 1;
        public const int FALSE = 0;

        //#define READ_TEXT    "r"
        //#define READ_BINARY  "r"
        //#define WRITE_TEXT   "w"
        //#define WRITE_BINARY "w"
        //#define REGISTER register
        public const string READ_TEXT    = "r";
        public const string READ_BINARY  = "r";
        public const string WRITE_TEXT   = "w";
        public const string WRITE_BINARY = "w";
        public const int EOF = -1;
        /*
          #define REGISTER_DOUBLE(i) double i = 19700217.0
          */
        //#define REGISTER_DOUBLE(i) double i

        /* definition of new variable type */
        //typedef unsigned int POINTER;

        //#define PI 3.1415926535897932384626
        //#define RADIAN (PI / 180.0)
        //#define INFINITY (1E+20)
        //#define XX 1
        //#define YY 2
        //#define ZZ 3
        //#define FULL   (-1)
        //#define MIDDLE   0
        //#define EMPTY    1
        public const double PI = 3.1415926535897932384626;
        public const double RADIAN = PI / 180.0;
        public const double INFINITY = 1E+20;
        public const int XX = 1;
        public const int YY = 2;
        public const int ZZ = 3;
        public const int FULL = -1;
        public const int MIDDLE = 0;
        public const int EMPTY = 1;

        /* check sign of value macro */
        //#define SGN(i)      ((i) <= 0 ? -1 : 1)
        //#define NEGATIVE(i) ((i) <= 0)
        public static int SGN(double i)
        {
            return ((i) <= 0.0 ? -1 : 1);
        }
        public static bool NEGATIVE(double i)
        {
            //return ((i) <= 0 ? TRUE : FALSE);
            return (i <= 0.0);
        }

        /* definition of scm */
        public class tag_scm
        {
            public uint label;
            public int type;
            public double r, g, b;
            public double fd;
            public double fh;
            public int hm;
            public double hc;
            public double fs1;
            public double fs2;
            public double ka, kb, kc;
            public double n;
            public int source_no;
            public class tag_mapping {
                //FILE* fp;
                public FileStream fp;
                public uint[] scm = new uint[2];     /* pointer to scm[] */
                public uint[] color = new uint[2];
                public double x, y;
                public double dx, dy;
                public int mapping_no;
                public double rb, rp, rh;
                public double[] gmat = new double[10];        /* Invers matrix of rotation. */
                public double ox, oy, oz;
                public double sclx, scly, sclz;    /* Scaling factor */
                public byte[] mem;
                public int fx, fy;
            };
            public tag_mapping mapping = new tag_mapping();
            public double[,] fac = new double[0,3];     // [,3]
            public double[][,] f = new double[0][,];    // [][10,3]
            public double[,] d = new double[0,3];       // [,3]
            public double[][,] rf = new double[0][,];   // [][10,3]
        }
        public static tag_scm[] scm;

        //#define MAX_SOURCE_NO  9
        //#define MAX_MAPPING_NO 2
        public const int MAX_SOURCE_NO = 9;
        public const int MAX_MAPPING_NO = 2;

        //extern POINTER use_scm, max_scm;
        public static uint use_scm, max_scm;

        /* definition of prim */
        public class tag_prim
        {
            public int kind;
            public double cx, cy, cz;
            public double sg;
            public uint scm;
            public double[] par = new double[20];
            public double[] mat = new double[10];
            public ulong count;
            public uint pdno;
        }

        public static tag_prim[] prim;
        //#define PRIM(n) prim[n]
        public static tag_prim PRIM(in uint n)
        {
            return prim[n];
        }

        //extern POINTER use_prim, max_prim;
        public static uint use_prim, max_prim;
        //extern POINTER max_meta_ball;
        public static uint max_meta_ball;
        //extern double (* stack1)[2];
        public static double[,] stack1 = new double[0, 2];
        //extern double (* stack2)[2];
        public static double[,] stack2 = new double[0, 2];
        //extern double (* stack3)[2];
        public static double[,] stack3 = new double[0, 3];
        //extern POINTER use_meta_list;
        public static uint use_meta_list;
        //extern POINTER appear_meta_list;
        public static uint appear_meta_list;
        //extern POINTER* meta_list;
        public static uint[] meta_list;

        /* definition of light */
        public class tag_light
        {
            public double cx, cy, cz;
            public double r, g, b;
            public double light;
        };
        public static tag_light[] light;
        //extern POINTER use_light, max_light;
        public static uint use_light, max_light;

        /* definition of view */
        public class tag_view
        {
            public double[] mat = new double[10];
            public double cx, cy, cz;
            public double scrx, scry, scrl;
            public int pxlx, pxly;
            public double k1, k2, k3, k4;
            public int anti;
            public double anti_aliasing_factor;
            public bool shadow;
            public double maxx, maxy, maxz;
            public double minx, miny, minz;
            public int nmin, nmax, pmax;
            public bool method;
            public bool trans;
            public string Object;
            public int branch_level;
            public int depth_level;
            public double limit_power;
            public double ts_factor;
        };
        public static tag_view view = new tag_view();

        public class CROSS_POINT {
            public double t;
            public double x, y, z;
            public uint primno;
            public bool flag;
            public double vx, vy, vz;
            public double r, g, b;
            public uint t_scm;
            public tag_scm t_scms = new tag_scm();
            public double nx, ny, nz;
            public uint scm;
            public tag_scm scms = new tag_scm();
        };

        /* definition of atable */
        //extern POINTER(*atable)[2];
        public static uint[,] atable = new uint[0, 2];
        //extern POINTER use_atable, max_atable;
        public static uint use_atable, max_atable;

        /* definition of btree */
        //extern POINTER* btree;
        public static uint[] btree;
        //extern POINTER use_btree, max_btree;
        public static uint use_btree, max_btree;

        /* definition of trans */
        //extern POINTER* trans;
        public static uint[] trans;
        //extern POINTER use_trans;
        public static uint use_trans;

        /* definition of pd */
        public class tag_pd
        {
            public uint label;
            public int kind;
            public double pa, pb, pc;
            public double rb, rp, rh;
            public double ox, oy, oz;
            public double sg;
            public uint scm;
            public double light;
            public int type;
            public double[] scl = new double[3];
        };
        public static tag_pd[] pd;

        /* definition of outprim */
        //extern POINTER* outprim, use_outprim;
        public static uint[] outprim;
        public static uint use_outprim;

        // Make C-styled stinrg converter.
        public static string C_String(char[] c)
        {
            string r = "";
            for (int i = 0; i < c.Length; i++)
            {
                if (c[i] == '\0') break;
                r += char.ToString(c[i]);
            }
            return r;
        }

        //public static System.Threading.SynchronizationContext EXRAYcontext;
        public static string dataPath;
    }
}
