#Usage: randomScraper.py
import sys
from scraper import *
from scraper import recipe

site = "https://www.themealdb.com/api/json/v1/1/random.php"
print(scrape(site))
