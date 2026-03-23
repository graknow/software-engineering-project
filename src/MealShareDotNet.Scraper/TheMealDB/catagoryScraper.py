#Usage: catagoryScraper.py catagorty
import sys
from scraper import *
from scraper import recipe
import argparse

parser = argparse.ArgumentParser(
                    prog='catagory Scraper',
                    description='Grabs recipes from TheMealDB based on the catagory they are in',
                    epilog='Stop reading this, it is a waste of your time')

parser.add_argument("name", help="The name of the catagory which we are searching recipes form")
args = parser.parse_args()

category = args.name

site = "https://www.themealdb.com/api/json/v1/1/filter.php?c=" + category
print(multiScrape(site))
