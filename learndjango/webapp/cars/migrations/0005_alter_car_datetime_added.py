# Generated by Django 4.1.1 on 2022-09-29 08:40

import datetime
from django.db import migrations, models


class Migration(migrations.Migration):

    dependencies = [
        ("cars", "0004_alter_car_datetime_added"),
    ]

    operations = [
        migrations.AlterField(
            model_name="car",
            name="datetime_added",
            field=models.DateTimeField(
                default=datetime.datetime(2022, 9, 29, 10, 40, 51, 787866)
            ),
        ),
    ]
