#Usage: nameScraper.py name
import sys
from scraper import *
from scraper import recipe
import argparse

parser = argparse.ArgumentParser(
                    prog='Name Scraper',
                    description='Grabs the recipe from TheMealDB based on the name of the recipe',
                    epilog='Stop reading this, it is a waste of your time')

parser.add_argument("name", help="The name we are searching for")
args = parser.parse_args()

name = args.name

site = "https://www.themealdb.com/api/json/v1/1/search.php?s=" + name
print(scrape(site))
