::  This is a simple automatization script
::  for testing, adding, pushing by one script running
::  first of all this tests check the correction of arguments, if missing stops the program
::  after that runs all tests, if test passed check internet connection
::  if connection exists, makes commit and push

@echo off
SETLOCAL EnableDelayedExpansion

SET "TestResult=test_output.txt"
SET "CommitResult=commit_output.txt"
SET "PushResult=push_output.txt"

IF "%1" NEQ "" (
    ECHO "[INFO] Start testing..."
    dotnet test > %TestResult%
    IF !ERRORLEVEL! EQU 0 (
        ECHO [INFO] Tests were succeed
        ECHO [INFO] Adding files to the git
        git add .
        ECHO [INFO] Checking internet connection
        ping google.com -n 1 -w 2000 > nul
        IF !ERRORLEVEL! EQU 0 (
            ECHO [INFO] Making commit
            git commit -m"%1" > %CommitResult%
            IF !ERRORLEVEL! EQU 0 (
                ECHO [INFO] Commit was succeed
                ECHO [INFO] Making push
                git push > %PushResult%
                if !ERRORLEVEL! EQU 0 (
                    ECHO [SUCCESS] Push was made
                ) ELSE (
                    ECHO [ERROR] Push error
                    ECHO Push output: %PushResult%
                )
            ) ELSE (
                ECHO [ERROR] Commit error
                ECHO Commit output: %CommitResult%
            )
        ) ELSE (
            ECHO [ERROR] No internet connection
        )
    ) ELSE (
        ECHO [ERROR] Tests was not succeed
        ECHO %TestResult%
    )
  
) ELSE (
    ECHO [FATAL] Commit message argument is missing
)

DEL "%TestResult%" "%CommitResult%" "%PushResult%" > nul 2>&1

PAUSE
ENDLOCAL 