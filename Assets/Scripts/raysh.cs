using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace EXRAY
{
    public class raysh {
        /*
         * Ray-Tracing header file II
         * Copyright by Mitsunori Satomi
         * $Author: satomi $ $Revision: 1.12 $ $Date: 88/09/22 18:22:42 $
         * $Header: rays.h,v 1.12 88/09/22 18:22:42 satomi Exp $
         */

        /* definition of macros */
        //#define LEN_TEMP 300
        //#define LEN_LABEL 30
        //#define BASE_LABEL ((POINTER)0x100)
        public const int LEN_TEMP = 300;
        public const int LEN_LABEL = 30;
        public const uint BASE_LABEL = 0x100;

        /* sizeof(POINTER) must be 4(bytes). */
        //#define BASE_VALUE ((POINTER) 0x80000000)
        //#define BASE_SEPA  ((POINTER) 0xffffff00)
        public const uint BASE_VALUE = 0x80000000;
        public const uint BASE_SEPA  = 0xffffff00;

        public const int P_EOF = 0;
        public const int P_COLOR = 1;
        public const int P_RGB = 2;
        public const int P_FD = 3;
        public const int P_FH = 4;
        public const int P_HM = 5;
        public const int P_HC = 6;
        public const int P_FS1 = 7;
        public const int P_FS2 = 8;
        public const int P_T0 = 9;
        public const int P_N = 10;
        public const int P_INIT = 11;
        public const int P_PRIMITIVE = 12;
        public const int P_KIND = 13;
        public const int P_PABC = 14;
        public const int P_RBPH = 15;
        public const int P_OXYZ = 16;
        public const int P_SG = 17;
        public const int P_SCM = 18;
        public const int P_LIGHT = 19;
        public const int P_TYPE = 20;
        public const int P_MODULE = 21;
        public const int P_WORLD = 22;
        public const int P_MAPPING = 23;
        public const int P_DEFINE = 24;
        public const int P_INCLUDE = 25;
        public const int P_SCALE = 26;
        public const int P_PREFIX = 27;
        public const int P_UNPREFIX = 28;
        public const int P_SOURCE = 29;
        public const int P_UNDEF = 30;
        public const int P_END = 31;

        /* definition of variables */
        //extern char (* label)[LEN_LABEL + 1];
        public static string[] label;
        //extern POINTER max_label, use_label;
        public static uint max_label, use_label;
        //extern double* value;
        public static double[] value;
        //extern POINTER max_value, use_value;
        public static uint max_value, use_value;
        //extern char* slabel[BASE_LABEL];
        public static string[] slabel = new string[BASE_LABEL];
        //extern POINTER use_slabel;
        public static uint use_slabel;
        //extern struct tag_pd *pd;
        //public static rayh.tag_pd[] pd;
        //extern POINTER max_pd, use_pd;
        public static uint max_pd, use_pd;
        //extern POINTER* macro;
        public static uint[] macro;

        //#define chklabel(i) ((i) < BASE_VALUE)
        public static bool chklabel(in uint i)
        {
            return ((i) < BASE_VALUE);
        }
        //#define chkvalue(i) (BASE_VALUE <= (i) && (i) < BASE_SEPA)
        public static bool chkvalue(in uint i)
        {
            return (BASE_VALUE <= (i) && (i) < BASE_SEPA);
        }
        //#define chksepa(i)  (BASE_SEPA <= (i))
        public static bool chksepa(in uint i)
        {
            return (BASE_SEPA <= (i));
        }
        //#define chkeof(i)   ((i) == P_EOF)
        public static bool chkeof(in uint i)
        {
            return ((i) == P_EOF);
        }
        //#define poilabel(i) ((i) < BASE_LABEL ? slabel[i] : label[(i) - BASE_LABEL])
        public static string poilabel(in uint i)
        {
            return ((i) < BASE_LABEL ? slabel[i] : label[(i) - BASE_LABEL]);
        }
        //#define chk01(i)    ((i) >= 0 && (i) <= 1.0)
        public static bool chk01(in double i)
        {
            return ((i) >= 0.0 && (i) <= 1.0);
        }
        //#define cntrl(i)    ((i) <= 0x20)
        public static bool cntrl(in char i)
        {
            return ((i) <= 0x20);
        }

        /* definition of TEMP */
        //#define TEMP_CORE_SIZE 2000
        public const int TEMP_CORE_SIZE = 2000;
        //#define TEMP_MAX_CORE  1000
        public const int TEMP_MAX_CORE = 1000;

        public class tag_TEMP {
            public uint[][] core = new uint[TEMP_MAX_CORE][];
            public uint max_core;
            public uint[] l_position;
            public uint position;
            public uint now_core;
            public uint counter;
        };
        public static tag_TEMP TEMP = new tag_TEMP();

        public class tag_TEMP_SUB {
            public uint[] l_position;
            public uint position;
            public uint now_core;
            public uint counter;
        };
        public static tag_TEMP_SUB TEMP_SUB;

        /* definition of external function and macros */
        //#define rword(tp)       (--(tp->counter)>0 ? *(tp->position++) : flush_rword(tp))
        public static uint rword(tag_TEMP tp)
        {
            return (--(tp.counter) > 0 ? tp.l_position[tp.position++] : ray0.flush_rword(tp));
        }
        //#define wword(tp, data) (--(tp->counter)>0 ? *(tp->position++) = (data) : (flush_wword(tp,data), 1))
        public static void wword(tag_TEMP tp, uint data)
        {
            if (--(tp.counter) > 0) {
                tp.l_position[tp.position++] = (data);
            } else {
                ray0.flush_wword(tp, data);
            }
        }

    }
}