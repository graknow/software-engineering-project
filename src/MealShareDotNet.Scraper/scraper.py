import requests
import json
import re
import yaml
import sys

class recipe:
    def __init__(self, name="", instructions="", ingredients=[], quantities=[]):
        self.name = name
        self.instructions = instructions
        self.ingredients = ingredients
        self.quantities = quantities

    def __repr__(self):
        return "%s\n %s\n %s " % (self.name, self.instructions, self.ingredients)

    def makeYaml(self):
        return {'name': self.name, 'instructions': self.instructions, 'ingredients': self.ingredients}

class ingredient(yaml.YAMLObject):
    yaml_tag = u'!Ingredient'
    scale = {
        "teaspoon": 50,
        "tablespoon": 148,
        "cup": 2366,
        "gallon": 37854,
        "pint": 4732,
        "pound": 45359,
        "liter": 1000,
        "milliliter": 10,
        "ml": 10,
        "ounce": 2835,
        "gram": 10,
        "kilogram": 10000,
        "kg": 10000
    }

    solid = ("pound", "ouce", "gram", "kilogram", "kg")
    fluid = ("teaspoon", "tablespoon", "cup", "gallon", "pint", "liter", "milliliter", "ml")

    def __init__(self, name="", quantity=None, mass=None, volume=None, parentID=None, quantityName=None):
        self.name = name
        self.parentID = parentID
        self.quantityName = quantityName.strip().split(" ")[-1]
        self.quantity = quantity
        self.mass = mass
        self.volume = volume

        if any(val in quantityName for val in self.solid):
            self.mass = self.normalize(quantity, quantityName)
            self.quantity = None

        elif any(val in quantityName for val in self.fluid):
            self.volume = self.normalize(self.quantity, self.quantityName)
            self.quantity = None

        else:
            self.quantity = quantity

    def __repr__(self):
        return "%s %s %s %s %s" % (self.name, self.quantity, self.mass, self.volume, self.quantityName)

    def normalize(self, quantity=0, quantityName=""):
        for val in self.scale.keys():
            if(quantityName.find(val) != -1):
                return float(quantity) * self.scale[val]
        return quantity

site = sys.argv[1]

response = requests.get(site)

recipe_json = json.loads(response.text)

recipe = recipe()
ingredients = []
quantities = []

for i in range(1, 20):
    Ingredient = recipe_json["meals"][0]["strIngredient" + str(i)]
    quantity = recipe_json["meals"][0]["strMeasure" + str(i)]
    if Ingredient != '' and Ingredient is not None:
        quantity_num = re.findall(r'\d+', quantity)
        if len(quantity_num) != 0:
            quantity = quantity.split(quantity_num[-1])
            quantity = quantity[-1]
        if len(quantity_num) == 2:
            quantity_num = int(quantity_num[0]) / int(quantity_num[1])
        else:
            quantity_num = '1'
        val = ingredient(Ingredient, quantity_num, None, None, None, quantity)
        ingredients.append(val)
        quantities.append(quantity)


recipe.ingredients = ingredients
recipe.quantities = quantities

recipe.name = recipe_json["meals"][0]["strMeal"]

recipe.instructions = recipe_json["meals"][0]["strInstructions"].replace("\n", "")

for i in range(1,20):
    quantity = recipe_json["meals"][0]["strMeasure" + str(i)]
    if quantity != '' and quantity is not None:
        quantities.append(quantity)

recipe.quantities = quantities

print(yaml.dump(recipe.makeYaml(), width = 1000))
