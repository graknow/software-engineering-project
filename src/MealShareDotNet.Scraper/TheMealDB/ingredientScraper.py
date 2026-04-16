#Usage: ingredientScraper.py ingredient
import sys
from scraper import *
from scraper import recipe
import argparse

parser = argparse.ArgumentParser(
                    prog='Ingredient Scraper',
                    description='Grabs the recipes from TheMealDB based on the main ingredient of the recipe',
                    epilog='Stop reading this, it is a waste of your time')

parser.add_argument("name", help="The name of the main ingredient we filtering by")
args = parser.parse_args()

ingredient = args.name

site = "https://www.themealdb.com/api/json/v1/1/filter.php?i=" + ingredient
print(multiScrape(site))
