#if defined(DC_VER_WINDOWS)
//#include <stdio.h>
//#include <windows.h>
#elif defined(DC_VER_LINUX)
#include <stdio.h>
#include <termio.h>
#include <fcntl.h>
#include <signal.h>
#include <ctype.h>
#include <errno.h>
#include <sys/types.h>
#include <unistd.h>
#include <stdlib.h>
#include <sys/stat.h>
#include <string.h>
#else
#define DC_VER_WINDOWS
#include <stdio.h>
#include <windows.h>
#endif

#if defined(DC_VER_LINUX)
#define _STD_FUNCTION 
#else
#define _STD_FUNCTION __stdcall
#endif

#include "DcType.h"
#include "DcStatus.h"
#include "DcSpecial.h"
