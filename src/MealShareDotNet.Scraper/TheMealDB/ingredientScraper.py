#Usage: ingredientScraper.py ingredient
import sys
from scraper import *
from scraper import recipe

ingredient = sys.argv[1]

site = "https://www.themealdb.com/api/json/v1/1/filter.php?i=" + ingredient
print(multiScrape(site))
