# gemini-client
I am presenting my thought process and things I was considering when writing this project. So it is clear why I ended up implementing solution this way.


# Order book problem
Typically order book model requires optimizing two things:
1. data structure to store different prices
2. data structure to store orders with this same price

At the beginning I was tempted to optimize 2, but I quickly realized that to be able to find the best bid and ask prices I don't really have to keep track of unique orders, I am just interested in a quantity per price. Even if I wanted to work on unique orders I don't have a unique ID of the order - which makes it hard to cancel an order w/o having a unique ID. 

# Typical models used in order book
1. Array
2. Linked list
3. B-Tree or Red Black tree
4. More complex approach 


# Optimizing a order book model
When thinking about choosing a good model I had following things in mind:
1. Which prices are most important for me
2. How is exchange behaving
3. What may be needed in the future when various autotrading algorithms are introduced

1. I am interested in best bid and ask price. I imagine that I am most interested in top few best prices and not everything else. It means that for large sets of data I may consider introducing a separate small data structure (fast cache) for best price, and make sure that it is up to date fast, and not focus on everything else. 