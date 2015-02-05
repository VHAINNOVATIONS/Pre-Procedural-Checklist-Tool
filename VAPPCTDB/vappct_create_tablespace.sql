CREATE TABLESPACE VAPPCT
DATAFILE 'C:\oraclexe\oradata\VAPPCT.DBF' 
SIZE 200m 
AUTOEXTEND ON 
NEXT 50M 
MAXSIZE UNLIMITED
/


create user VAPPCT identified by "password" quota unlimited on VAPPCT;
grant execute on SYS.DBMS_CRYPTO to VAPPCT;
grant execute on SYS.DBMS_LOB to VAPPCT;
grant execute on SYS.DBMS_LOCK to VAPPCT;
grant execute on SYS.DBMS_SCHEDULER to VAPPCT;
grant execute on SYS.DBMS_SQL to VAPPCT;
grant execute on SYS.UTL_FILE to VAPPCT;
grant execute on SYS.UTL_HTTP to VAPPCT;
grant select on SYS.V_$INSTANCE to VAPPCT;
grant select on SYS.V_$SESSION to VAPPCT;
grant connect to VAPPCT;
grant create table to VAPPCT;
grant create procedure to VAPPCT;
grant create sequence to VAPPCT;
grant create view to VAPPCT;
alter user VAPPCT quota unlimited on system;