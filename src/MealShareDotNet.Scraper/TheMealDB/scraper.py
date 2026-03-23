import requests
import json
import re
import yaml
import sys

class recipe(yaml.YAMLObject):
    yaml_tag = u'!Recipe'
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
        "tsp": 50,
        "tablespoon": 148,
        "tbs": 148,
        "cup": 2366,
        "gallon": 37854,
        "pint": 4732,
        "pound": 45359,
        "lb": 45359,
        "liter": 1000,
        "milliliter": 10,
        "ml": 10,
        "ounce": 2835,
        "gram": 10,
        "g": 10,
        "kilogram": 10000,
        "kg": 10000
    }

    solid = ("pound", "lb", "ouce", "gram", "g", "kilogram", "kg")
    fluid = ("teaspoon", "tsp", "tablespoon", "tbs", "cup", "gallon", "pint", "liter", "milliliter", "ml")

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
            self.quantity = self.normalize(self.quantity, self.quantityName)

    def __repr__(self):
        return "%s %s %s %s %s" % (self.name, self.quantity, self.mass, self.volume, self.quantityName)

    def normalize(self, quantity=0, quantityName=""):
        for val in self.scale.keys():
            if(quantityName.find(val) != -1 and val != "g"):
                return float(quantity) * self.scale[val]
            test = re.search("[0-9][0-9]g", quantityName)
            if(test):
                return float(quantity) * self.scale[val]
        return quantity

def parseJson(recipe_json):
    Recipe = recipe()
    ingredients = []
    quantities = []

    for i in range(1, 20):
        Ingredient = recipe_json["strIngredient" + str(i)]
        quantity = recipe_json["strMeasure" + str(i)]
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


    Recipe.ingredients = ingredients
    Recipe.quantities = quantities

    Recipe.name = recipe_json["strMeal"]

    Recipe.instructions = recipe_json["strInstructions"].replace("\n", "")

    for i in range(1,20):
        quantity = recipe_json["strMeasure" + str(i)]
        if quantity != '' and quantity is not None:
            quantities.append(quantity)

    Recipe.quantities = quantities
    return Recipe

def scrape(site):
    response = requests.get(site)

    recipe_json = json.loads(response.text)
    recipe = parseJson(recipe_json["meals"][0])

    print(yaml.dump(recipe.makeYaml(), width = 1000))

def multiScrape(site):
    response = requests.get(site)

    recipe_json = json.loads(response.text)
    recipes = []

    for item in recipe_json["meals"]:
        if len(item["strMeal"]) != 0:
            site = "https://www.themealdb.com/api/json/v1/1/search.php?s=" + item["strMeal"]
            response = requests.get(site)
            recipe_json = json.loads(response.text)
            recipes.append(parseJson(recipe_json["meals"][0]))
            # print(parseJson(recipe_json["meals"][0]))
    yamls = {}
    for recipe in recipes:
        yamls[recipe.name] = recipe.makeYaml()
    return yaml.dump(yamls, width = 1000)
    # print(yaml.dump(recipes.makeYaml(), width = 1000))

