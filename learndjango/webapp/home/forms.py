from dataclasses import fields
from django import forms
from .models import Signup


# Creating Signup form with the forms class.
class SignupForm(forms.Form):
    # params for form field => label, initial, disabled, help_text, widget, required
    username = forms.CharField(max_length=20)
    password = forms.CharField(max_length=20, widget=forms.PasswordInput)


# For this class we are using another approach to create form content,
# by creating subclass Meta then define which model we are going to use and fields we want to display.
class SigninForm(forms.ModelForm):

    class Meta:
        model = Signup
        # __all__ selects all class fields
        # fields = '__all__'
        fields = ['username', 'password']
