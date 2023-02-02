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
    dag_id = "[NODE_ID_ENV]",
    default_args=args,
    schedule_interval=timedelta(hours=1),
    dagrun_timeout=timedelta(minutes=60),
    description='[NODE_DESC_ENV]',
    start_date = datetime(2023, 2, 2),
    catchup=False,
)

create_table_sql_query = '''
[QUERY_ENV];
'''

create_table = PostgresOperator(
sql = create_table_sql_query,
task_id = "[NODE_TASK_ENV]",
postgres_conn_id = "[CREDENTIAL_ID_ENV]",
dag = dag_psql
)