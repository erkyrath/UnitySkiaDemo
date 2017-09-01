#!/usr/bin/env python

# This is a very simple utility which copies the SkiaSharp libraries
# (DLL files) into the correct place in a freshly-build Unity app.
# This works on Mac and Windows app packages.
#
# Usage:
#    python putlibs.py UNITYAPP...
# (where UNITYAPP is either an xxx.app directory or an xxx_Data directory
# built by Unity.)
#
# The script assumes that it is being run from the SkiaComponents
# directory, so that the managed and native folders are right next to it.

import sys
import os
import os.path
import shutil
import subprocess

arglist = sys.argv[ 1 : ]

if not arglist:
    print('Usage: python putlibs.py UNITYAPP...')
    sys.exit(1)

homedir = os.path.dirname(sys.argv[0])
if not homedir:
    homedir = '.'

def test64bit(file):
    res = subprocess.check_output(['/usr/bin/file', file])
    if 'PE32 executable' in res:
        return False
    if 'PE32+ executable' in res:
        return True
    raise Exception('Cannot determine Windows app type: ' + file)
    
for dir in arglist:
    if dir.endswith('/'):
        dir = dir[ : -1 ]
        
    typ = None
    if dir.endswith('.app'):
        typ = 'mac'
    elif dir.endswith('_Data'):
        typ = 'win'
        exefile = dir[:-5]+'.exe'
        if test64bit(exefile):
            typ += '-x64'
        else:
            typ += '-x86'
    elif dir.endswith('.exe'):
        typ = 'win'
        exefile = dir
        dir = dir[:-4]+'_Data'
        if test64bit(exefile):
            typ += '-x64'
        else:
            typ += '-x86'
    else:
        raise Exception('Unrecognized app type: ' + dir)
    
    print('Fixing up %s (%s)...' % (dir, typ))

    if typ == 'mac':
        srcfile = os.path.join(homedir, 'native', 'mac', 'libSkiaSharp.dylib')
        destdir = os.path.join(dir, 'Contents', 'Frameworks', 'MonoEmbedRuntime', 'osx')
        if not os.path.exists(destdir):
            os.makedirs(destdir)
        shutil.copy(srcfile, destdir)
            
        #srcfile = os.path.join(homedir, 'managed', 'mac', 'SkiaSharp.dll')
        #destdir = os.path.join(dir, 'Contents', 'Resources', 'Data', 'Managed')
        #shutil.copy(srcfile, destdir)
        
    elif typ.startswith('win'):
        srcfile = os.path.join(homedir, 'native', typ, 'libSkiaSharp.dll')
        destdir = os.path.join(dir, 'Mono')
        if not os.path.exists(destdir):
            os.makedirs(destdir)
        shutil.copy(srcfile, destdir)
            
        #srcfile = os.path.join(homedir, 'managed', 'win', 'SkiaSharp.dll')
        #destdir = os.path.join(dir, 'Managed')
        #shutil.copy(srcfile, destdir)

