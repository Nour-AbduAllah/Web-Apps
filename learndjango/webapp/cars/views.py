from django.shortcuts import render
from .models import Car


# Create your views here.
def index(request):
    context = {'page_title': 'Cars'}
    return render(request, 'cars/index.html', context)
    pass


def car(request):
    carname = 'Ds Survolt'

    car = Car.objects.get(id=2)

    context = {'car': car, 'page_title': car.name + ' details'}
    return render(request, 'cars/car.html', context)
    pass


def cars(request):
    collectedcars = Car.objects.all().filter(price__gte=30000)
    context = {
        # field lookups => __gte(greater than or equal), __lte(less than or equal), __exact(=),
        # __iexact(insensitive match), __contains, __startwith, __endwith, __istart/endwith,
        # tablename__fieldname(for joins), __isnull, __year(works with date type), __in(values in list[]),
        # __range(start, end)
        'cars': collectedcars.order_by('price'),
        # we sould pass an iterable object to the template, so we should convert int to string here.
        'carscount': str(collectedcars.count()),
        'page_title': 'Cars',
    }
    return render(request, 'cars/cars.html', context)
    pass
