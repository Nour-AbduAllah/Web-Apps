from datetime import date
from xmlrpc.client import DateTime
from django import http
from django.shortcuts import render
from django.http import HttpResponse
from .models import *
from .forms import SigninForm, SignupForm


# Create your views here.
def home(request):
    page_title = 'Home'
    # return HttpResponse('<h1>Dear Nour:</br>You are Welcome, this is your 1st django project<h1\>.')
    return render(request, 'home/home.html', {'page_title': page_title})
    pass


def about(request):
    page_title = 'About us'
    # return HttpResponse(
    #     '<h3>This is a project to learn how to use python-django in web applications<h3\>.'
    # )
    return render(request, 'home/about.html', {'page_title': page_title})
    pass


def contact(request):
    page_title = 'Contact us'
    # content = """<div><h4>Name: </h4>{{name}}</div>
    #     <div><h4>Email: </h4>{{email}}</div>
    #     <div><h4>Phone:</h4> {{phone}}</div>
    #     <div><h4>Address:</h4> {{address}}</div>
    #                 """
    # return HttpResponse(content)
    context = {
        'name': 'Nour AbdAllah Mohammed',
        'email': 'nourabdallah054@gmail.com',
        'phone': '+20 10 972 33 758',
        'address': 'Egypt-1st floor',
        'page_title': page_title,
    }
    return render(request, 'home/contact.html', context)
    pass


def signup(request):
    name = request.POST.get('username')
    password = request.POST.get('password')

    data = Signup(username=name, password=password)
    print(data)
    if request.method is 'POST' and data.is_valid():
        data.save()
    context = {
        'page_title': 'SignUp',
        'data': data,
        'signupform': SignupForm,
    }
    return render(request, 'home/signup.html', context)
    pass


def signin(request):
    data = SigninForm(request.POST)
    print(data)
    context = {
        'signinform': SigninForm,
        'page_title': 'Sign-in',
    }
    # check if data from form is in db, if correct got to home else ask for data again.
    return render(request, 'home/signin.html', context)