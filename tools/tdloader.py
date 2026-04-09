#!/usr/bin/env python

import argparse
import datetime
import os
import sqlite3
import yaml


def get_columns(rows: list[dict]) -> list[str]:
    columns = []
    for row in rows:
        for col in list(row.keys()):
            if col not in columns:
                columns.append(col)

    return columns


def get_value_entry(row: dict, columns: list[str]) -> list[str]:
    value = []
    for column in columns:
        v = row.get(column)

        if v is None:
            v = "NULL"
        elif isinstance(v, str):
            if v.startswith("$$"):
                v = f"'{parse_datetime(v[2:])}'"
            else:
                v = f"'{v}'"
        else:
            v = str(v)
        value.append(v)

    return value


def parse_datetime(data: str) -> datetime.datetime:
    today = datetime.datetime.now()

    (day_offset, hour, minute) = [int(s) for s in data.split(" ")]

    return today.replace(day=today.day + day_offset, hour=hour, minute=minute, second=0, microsecond=0)


def wipe_table(db_path: str, table_name: str):
    sql = "DELETE FROM " + table_name

    db = sqlite3.connect(db_path)

    try:
        db.execute(sql)
        db.commit()

        print(f"Successfully wiped table \"{table_name}\"")
    except:
        print(f"An error ocurred when wiping table \"{table_name}\"")
        db.rollback()
    finally:
        db.close()


def main(data_dir: str, db_path: str, wipe: bool) -> int:
    data_files = [ os.path.join(data_dir, f) \
            for f in os.listdir(data_dir) \
            if os.path.isfile(os.path.join(data_dir, f)) ]

    for file in data_files:
        table_name = os.path.splitext(os.path.basename(file))[0]

        if wipe:
            wipe_table(db_path, table_name)

        rows = {}
        with open(file, 'r') as f:
            rows = yaml.safe_load(f)

        columns = get_columns(rows)

        values = []
        for row in rows:
            values.append(get_value_entry(row, columns))

        sql = "INSERT INTO " + table_name
        sql += " (" + ",".join(columns) + ") VALUES "

        for v in values:
            sql += "(" + ",".join(v) + "),"

        sql = sql.rstrip(",") + ";"

        db = sqlite3.connect(db_path)

        try:
            db.execute(sql)
            db.commit()
            print(f"Successfully appended data to table \"{table_name}\"")
        except Exception as e:
            print(f"An error ocurred when executing for data file \"{file}\": {repr(e)}")
            db.rollback()
        finally:
            db.close()

    return 0


if __name__ == "__main__":
    args = argparse.ArgumentParser(description="Load test data into the specified SQL database")

    args.add_argument("-d", "--data-dir", dest="data_dir", required=False,
                      default=os.path.join(os.getcwd(), "test-data" + os.path.sep + "tables"),
                      help="Directory with data to import")

    args.add_argument("-w", "--wipe-tables", dest="wipe", action="store_true", required=False,
                      default=False,
                      help="Wipe table contents before applying test data")

    args.add_argument("db_path",
                      help="Path to the database file")

    a = args.parse_args()

    ret_code = main(a.data_dir, a.db_path, a.wipe)

    exit(ret_code)
