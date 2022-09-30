from django.urls import path
from django.views import View
from . import views

urlpatterns = [
    path('', views.index, name='index'),
    path('car', views.car, name='car'),
    path('cars', views.cars, name='cars'),
]
