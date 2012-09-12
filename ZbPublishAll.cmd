@echo off
echo ********************************************************************************
echo Publish the basic modules for committing and deployment.
echo This updates the Modules and generated code in the source directory.
echo Use this to publish local changes in the basic modules.
echo ********************************************************************************

set config=

if .%1. == .. GOTO GOON

set config=%1

:GOON

cd bin\Debug

Zetbox.Server.Service.exe %config% -generate -updatedeployedschema -repairschema
IF ERRORLEVEL 1 GOTO FAIL

rem publish schema data for Workflow project
rem no config yet ;Workflow.Config
Zetbox.Server.Service.exe %config% -publish ..\..\Modules\Workflow.xml -ownermodules Workflow
IF ERRORLEVEL 1 GOTO FAIL

rem export Workflow.Config data
rem Zetbox.Server.Service.exe %config% -export ..\..\Data\Workflow.Config.xml -schemamodules Workflow.Config
rem IF ERRORLEVEL 1 GOTO FAIL

rem export Workflow.Data data
rem Zetbox.Server.Service.exe %config% -export ..\..\Data\Workflow.Data.xml -schemamodules Workflow -ownermodules Workflow
rem IF ERRORLEVEL 1 GOTO FAIL

echo ********************************************************************************
echo ************************************ Success ***********************************
echo ********************************************************************************
cd ..\..
GOTO EOF

:FAIL
echo XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
echo XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX FAIL XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
echo XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX
echo                                Aborting Publish
cd ..\..
rem return error without closing parent shell
echo A | choice /c:A /n

:EOF
pause
