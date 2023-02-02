import airflow
from datetime import datetime, timedelta
from airflow import DAG
from airflow.operators.postgres_operator import PostgresOperator
from airflow.utils.dates import days_ago

args={'owner': 'airflow'}

default_args = {
    'owner': 'airflow',    
    'retries': 1,
    'retry_delay': timedelta(minutes=5),
}

dag_psql = DAG(
    dag_id = "nd_postgre_40_1134770044",
    default_args=args,
    schedule_interval=timedelta(hours=1),
    dagrun_timeout=timedelta(minutes=60),
    description='fetch data from postgreSQL with specific credentials',
    start_date = datetime(2023, 2, 2),
    catchup=False,
)

create_table_sql_query = '''
CREATE TABLE temp_nd_postgre_40_1134770044 AS SELECT 
  we.id as workflowId, 
  we.name as workflow_name, 
  we.nodes, 
  we.connections, 
  we."createdAt", 
  ee."mode" as workflow_mode, 
  ee."startedAt" as last_started, 
  ee."stoppedAt" as last_actived 
FROM 
  workflow_entity we 
  join execution_entity ee on ee."workflowId" = we.id
;
'''

create_table = PostgresOperator(
sql = create_table_sql_query,
task_id = "postgre_nd_postgre_40_1134770044",
postgres_conn_id = "ut",
dag = dag_psql
)