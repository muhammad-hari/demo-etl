from airflow import DAG
from airflow.operators.python_operator import PythonOperator
import psycopg2
import pandas as pd
import datetime

def get_data_from_postgres(ds, **kwargs):
    conn = psycopg2.connect(
        host=kwargs["host"],
        database=kwargs["database"],
        user=kwargs["user"],
        password=kwargs["password"]
    )

    cursor = conn.cursor()

    table_name = kwargs["table_name"]
    query = f"[QUERY_INJECT]"
    cursor.execute(query)

    rows = cursor.fetchall()
    df = pd.DataFrame(rows)

    # Do some data processing or saving to another location
    df.to_csv(f"{table_name}_data.csv")

def create_dag(dag_id, schedule, default_args, host, database, user, password, table_name):
    dag = DAG(
        dag_id=dag_id,
        default_args=default_args,
        schedule_interval=schedule,
    )

    get_data_task = PythonOperator(
        task_id=f"get_data_from_{table_name}",
        python_callable=get_data_from_postgres,
        op_kwargs={
            "host": host,
            "database": database,
            "user": user,
            "password": password,
            "table_name": table_name
        },
        dag=dag,
    )

    return dag

default_args = {
    'owner': 'airflow',
    'depends_on_past': False,
    'start_date': datetime.datetime(2021, 1, 1),
    'email_on_failure': False,
    'email_on_retry': False,
    'retries': 1,
    'retry_delay': datetime.timedelta(minutes=5),
}


host = "27.112.79.107"
database = "n8n"
user = "postgres"
password = "postgres"

table1_dag = create_dag(
    dag_id="get_data_from_table1_dag",
    schedule=datetime.timedelta(hours=1),
    default_args=default_args,
    host=host,
    database=database,
    user=user,
    password=password,
    table_name="table1"
)

table2_dag = create_dag(
    dag_id="get_data_from_table2_dag",
    schedule=datetime.timedelta(hours=1),
    default_args=default_args,
    host=host,
    database=database,
    user=user,
    password=password,
    table_name="table2"
)