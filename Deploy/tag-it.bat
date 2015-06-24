set SVNBIN=\\foster\data\SVN_BIN\svn
set SVNPATH=http://svn:8080/svn/Foster-Box/
@ECHO Taging publish
del .\pub\box-version.txt
%SVNBIN% log %SVNPATH% >> .\pub\box-version.txt

