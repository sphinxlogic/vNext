# Common utility functions used by various script execution tests
#  e.g. test_cmd_line, test_cmd_line_script and test_runpy

import sys
import os
import os.path
import tempfile
try:
    import subprocess
except ImportError:
    subprocess = None
import py_compile
import contextlib
import shutil
import zipfile

if subprocess:
    # Executing the interpreter in a subprocess
    def python_exit_code(*args):
        cmd_line = [sys.executable, '-E']
        cmd_line.extend(args)
        with open(os.devnull, 'w') as devnull:
            return subprocess.call(cmd_line, stdout=devnull,
                                    stderr=subprocess.STDOUT)
    
    def spawn_python(*args, **kwargs):
        cmd_line = [sys.executable, '-E']
        cmd_line.extend(args)
        return subprocess.Popen(cmd_line, stdin=subprocess.PIPE,
                                stdout=subprocess.PIPE, stderr=subprocess.STDOUT,
                                **kwargs)

    def kill_python(p):
        p.stdin.close()
        data = p.stdout.read()
        p.stdout.close()
        # try to cleanup the child so we don't appear to leak when running
        # with regrtest -R.
        p.wait()
        subprocess._cleanup()
        return data

    def run_python(*args, **kwargs):
        if __debug__:
            p = spawn_python(*args, **kwargs)
        else:
            p = spawn_python('-O', *args, **kwargs)
        stdout_data = kill_python(p)
        return p.wait(), stdout_data

# Script creation utilities
@contextlib.contextmanager
def temp_dir():
    dirname = tempfile.mkdtemp()
    dirname = os.path.realpath(dirname)
    try:
        yield dirname
    finally:
        shutil.rmtree(dirname)

def make_script(script_dir, script_basename, source):
    script_filename = script_basename+os.extsep+'py'
    script_name = os.path.join(script_dir, script_filename)
    script_file = open(script_name, 'w')
    script_file.write(source)
    script_file.close()
    return script_name

def compile_script(script_name):
    py_compile.compile(script_name, doraise=True)
    if __debug__:
        compiled_name = script_name + 'c'
    else:
        compiled_name = script_name + 'o'
    return compiled_name

def make_zip_script(zip_dir, zip_basename, script_name, name_in_zip=None):
    zip_filename = zip_basename+os.extsep+'zip'
    zip_name = os.path.join(zip_dir, zip_filename)
    zip_file = zipfile.ZipFile(zip_name, 'w')
    if name_in_zip is None:
        name_in_zip = os.path.basename(script_name)
    zip_file.write(script_name, name_in_zip)
    zip_file.close()
    #if test.test_support.verbose:
    #    zip_file = zipfile.ZipFile(zip_name, 'r')
    #    print 'Contents of %r:' % zip_name
    #    zip_file.printdir()
    #    zip_file.close()
    return zip_name, os.path.join(zip_name, name_in_zip)

def make_pkg(pkg_dir):
    os.mkdir(pkg_dir)
    make_script(pkg_dir, '__init__', '')

def make_zip_pkg(zip_dir, zip_basename, pkg_name, script_basename,
                 source, depth=1, compiled=False):
    unlink = []
    init_name = make_script(zip_dir, '__init__', '')
    unlink.append(init_name)
    init_basename = os.path.basename(init_name)
    script_name = make_script(zip_dir, script_basename, source)
    unlink.append(script_name)
    if compiled:
        init_name = compile_script(init_name)
        script_name = compile_script(script_name)
        unlink.extend((init_name, script_name))
    pkg_names = [os.sep.join([pkg_name]*i) for i in range(1, depth+1)]
    script_name_in_zip = os.path.join(pkg_names[-1], os.path.basename(script_name))
    zip_filename = zip_basename+os.extsep+'zip'
    zip_name = os.path.join(zip_dir, zip_filename)
    zip_file = zipfile.ZipFile(zip_name, 'w')
    for name in pkg_names:
        init_name_in_zip = os.path.join(name, init_basename)
        zip_file.write(init_name, init_name_in_zip)
    zip_file.write(script_name, script_name_in_zip)
    zip_file.close()
    for name in unlink:
        os.unlink(name)
    #if test.test_support.verbose:
    #    zip_file = zipfile.ZipFile(zip_name, 'r')
    #    print 'Contents of %r:' % zip_name
    #    zip_file.printdir()
    #    zip_file.close()
    return zip_name, os.path.join(zip_name, script_name_in_zip)
