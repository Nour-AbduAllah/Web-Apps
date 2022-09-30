from pyexpat import model
from random import choices
from tabnanny import verbose
from django.db import models
from datetime import datetime


# Create your models here.
class Car(models.Model):

    # colors category for color dropdownlist
    colors = [
        # (db value, displayed value)
        ('not selected', 'Not Selected'),
        ('red', 'Red'),
        ('black', 'Black'),
        ('White', 'White'),
    ]
    # id field created by default.
    # id = models.IntegerField(primary_key=True)

    name = models.CharField(max_length=25)

    price = models.DecimalField(max_digits=10, decimal_places=3)

    describtion = models.TextField(null=True, blank=True)

    # uploading to photos directory and subdirectories to specify in which year and month and day this img added
    img = models.ImageField(upload_to='photos/%y/%m/%d',
                            verbose_name='picture')

    active = models.BooleanField(default=True)

    color = models.CharField(max_length=15,
                             default='Not Selected',
                             choices=colors)

    datetime_added = models.DateTimeField(default=datetime.now())

    def __str__(self):
        # This is a tostring() mathod overridden.
        return f'{self.id} : {self.name} {self.price}'
        pass

    class Meta:
        # This is a class of meta about Car class.

        # Here we are using verbose_name to controle viewed name in admin page.
        verbose_name = "Car"

        # Here we are ordering displayed data
        # to sore in descending order put - before the column name => '-id' '-name'
        ordering = ['name', 'price']
