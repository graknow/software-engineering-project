#Usage: areaScraper.py area
import sys
from scraper import *
from scraper import recipe
import argparse

parser = argparse.ArgumentParser(
                    prog='Area Scraper',
                    description='Grabs recipes from TheMealDB based on the area they originate from',
                    epilog='Stop reading this, it is a waste of your time')

parser.add_argument("name", help="The name of the area which we are searching recipes for")
args = parser.parse_args()

area = args.name

site = "https://www.themealdb.com/api/json/v1/1/filter.php?a=" + area
print(site)
print(multiScrape(site))
