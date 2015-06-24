set SVNBIN=\\foster\data\SVN_BIN\svn
set SVNPATH=https://github.com/foster-hub/box-cms.git
@ECHO Taging publish
del .\pub\box-version.txt
%SVNBIN% log %SVNPATH% >> .\pub\box-version.txt

