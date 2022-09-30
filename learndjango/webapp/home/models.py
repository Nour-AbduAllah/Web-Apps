from unittest.util import _MAX_LENGTH
from django.db import models


# Create your models here.
class Signup(models.Model):
    username = models.CharField(max_length=20)
    password = models.CharField(max_length=20)

    def __str__(self):
        if self.username is not None:
            return self.username
        else:
            return ''
