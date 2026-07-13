




Claim offer

Today 4:30 PM
can you make a union extension method in C# that does the same job as python's union?

Yep. If you want Python's set.union() semantics (unique elements, preserving the first occurrence in practice), here's the LINQ equivalent.

public static class EnumerableExtensions
{
    public static IEnumerable<T> UnionAll<T>(
        this IEnumerable<T> source,
        params IEnumerable<T>[] others)
    {
        var seen = new HashSet<T>();

        foreach (var item in source)
            if (seen.Add(item))
                yield return item;

        foreach (var collection in others)
        {
            foreach (var item in collection)
            {
                if (seen.Add(item))
                    yield return item;
            }
        }
    }
}
Usage:

var a = new[] { 1, 2, 3 };
var b = new[] { 3, 4, 5 };
var c = new[] { 5, 6, 7 };

var result = a.UnionAll(b, c).ToList();

// 1,2,3,4,5,6,7
If you're after Python's dict | dict behavior (right-hand side overwrites duplicate keys), here's that version:

public static class DictionaryExtensions
{
    public static Dictionary<TKey, TValue> Union<TKey, TValue>(
        this IDictionary<TKey, TValue> first,
        params IDictionary<TKey, TValue>[] others)
        where TKey : notnull
    {
        var result = new Dictionary<TKey, TValue>(first);

        foreach (var dict in others)
        {
            foreach (var kv in dict)
            {
                result[kv.Key] = kv.Value;
            }
        }

        return result;
    }
}
Usage:

var a = new Dictionary<string, int>
{
    ["a"] = 1,
    ["b"] = 2
};

var b = new Dictionary<string, int>
{
    ["b"] = 42,
    ["c"] = 3
};

var merged = a.Union(b);

// { a=1, b=42, c=3 }
If your goal is exactly Python's set.union(), you can even make it return a HashSet<T>:

public static HashSet<T> Union<T>(
    this IEnumerable<T> source,
    params IEnumerable<T>[] others)
{
    var set = new HashSet<T>(source);

    foreach (var other in others)
        set.UnionWith(other);

    return set;
}
This version is the closest semantic match to Python's set.union(): it returns a new set without modifying the original.


Today 9:54 PM
d2ddc991-7590-4766-93fa-657c150a59bb.png
Great.  my friend is claiming Dart 3 prevents all or most nulls.  How is it doing so?  Option<T> alone won't do it.  He also claims "many languages" successfully prevent nulls, and yet he wrote a massive httpResult wrapper-library in C# with a bunch of paradigm and KT required to use it properly.

I tried for hours to get him to explain himself, but all I got was his stupid nuget package.

20MB to prevent nulls from http requests, and his usage is a bloody switch statement that could have been a bloody enum (set by a raw httpcode) or better yet, a smart enum-class like the classic Microsoft Enumeration , or perhaps an implicit conversion from a null/bad httpcode to a specialized, pared-down Smart enum (ardalis).

ardalis/SmartEnum

Anything but a bloody Lsp, Mcp server and sourcegenerator just to eliminate a single slice (http) that none asked for.

It's over- architecture like this that drives me bonkers.

I think there are two separate questions here:

How do languages like Dart actually prevent nulls?

Is a giant HTTP result library the right way to model HTTP failures in C#?

Those have very different answers.

Dart isn't using Option<T>
Dart 3 (like Kotlin, Swift, Rust, modern C#, F#, etc.) uses the type system.

String name = "Nick";   // cannot be null
String? nick = null;    // nullable
If you write

print(name.length);
it's always safe.

If you write

print(nick.length);
the compiler refuses.

You must do

print(nick?.length);
or

if (nick != null) {
    print(nick.length);
}
or

print(nick!.length);
where ! means I promise this isn't null. If you're wrong, that's on you.

So the compiler tracks nullability everywhere.

C# already does almost the same thing
Since C# 8:

string name;
string? nickname;
The compiler performs flow analysis.

if (nickname != null)
{
    Console.WriteLine(nickname.Length);
}
After the null check, nickname is treated as non-null.

It's surprisingly sophisticated.

Option<T> solves a different problem
Option<T> isn't primarily about null.

It's about expressing

there might not be a value.

Instead of

User? user;
you have

Option<User>
which forces callers to acknowledge the absence case.

Rust's

Option<T>
is the canonical example.

HTTP is another different problem
HTTP has many outcomes.

200 OK
201 Created
204 No Content

400 Bad Request

401 Unauthorized

403 Forbidden

404 Not Found

409 Conflict

422 Validation

429 Too Many Requests

500 Server Error
Those aren't "null".

They're different states.

So a Result type can be useful.

For example

Result<User, ApiError>
is perfectly reasonable.

Where people go overboard
The screenshot is doing this

switch (user)
{
    case OkUser(var success):
        ...

    case ErrorUser(var error):
        ...
}
This is essentially a discriminated union.

Languages like

F#

Rust

Swift

Kotlin

have this built in.

C# doesn't.

So libraries fake it.

Is it necessary?
Depends.

If you're writing

var user = await client.GetUser();

if (user == null)
then yes, you've lost information.

Was it

timeout?

DNS?

unauthorized?

404?

validation?

malformed JSON?

Null tells you nothing.

But...

If your code immediately does

switch(result)
{
    case OkUser ...
    case ErrorUser ...
}
for every endpoint...

you've basically reinvented HTTP.

Personally, I'd lean toward
public sealed class ApiResponse<T>
{
    public HttpStatusCode StatusCode { get; init; }

    public T? Value { get; init; }

    public ProblemDetails? Error { get; init; }

    public bool Success => (int)StatusCode < 300;
}
or

Result<T, ApiError>
That's enough for most APIs.

No source generator.

No 20 MB package.

No generated

OkUser
ErrorUser
CreatedUser
DeletedUser
types.

SmartEnum?
I don't think SmartEnum is the right abstraction here.

SmartEnum is fantastic for things like

OrderStatus
or

UserRole
where the values are part of your domain.

HTTP status codes are already standardized.

The CLR already has

HttpStatusCode
which is effectively the enum you want.

If you need richer behavior, extension methods are usually enough:

public static bool IsSuccess(this HttpStatusCode code)
    => (int)code is >= 200 and < 300;

public static bool IsRetryable(this HttpStatusCode code)
    => code is
        HttpStatusCode.RequestTimeout or
        HttpStatusCode.TooManyRequests or
        HttpStatusCode.BadGateway or
        HttpStatusCode.ServiceUnavailable;
That keeps the HTTP layer simple while letting your domain layer decide what those statuses mean.

From what you've described, it sounds like your frustration isn't with the idea of modeling success and failure—it's with the amount of machinery involved. Source generators, an LSP, an MCP server, dozens of generated wrapper types, and a custom protocol can absolutely feel disproportionate if all you need is "deserialize on 2xx, otherwise give me the status code and error payload."

That's a trade-off. Some teams value exhaustive compile-time modeling enough to accept the complexity. Others prefer a thinner client and simpler debugging. Neither approach is inherently wrong, but the extra infrastructure should justify itself by reducing bugs or maintenance costs. If it doesn't, then it's reasonable to question whether the architecture is pulling its weight.


I like your solution for both error codes and your starter api.  that's sufficient for most things.

"you've basically reinvented HTTP."

Bingo.  The man doesn't realize that's what he's doing.  He remarked that c# lacked DU's.  I've never actually *needed* DU's in my life (I still have no idea why people are so hung up on them when C# was not intended to be a functional language.   I tried getting him to explain, but got unhelpful, generic answers).

Thanks for explaining the Dart, but C# does have a ?.  The subtle difference is that C# doesn't enforce use of ? or !, while Dart might - is that fair to say?

I use ? a lot for string lengths, but when I have control (developer say) over strings, I default their setters to string.Empty anytime I can get my hands on the "dto" or entity or whatever.

This is because most .NET devs are dumb/fearful and follow Microsoft's (terrible or uncaring) recommendations, instead of using their grey matter and coming up with a solution using the very AWESOME language they were given!

I see why he's attracted to Dart, but I'm not about to hop on board.  Neither is the Navy.  His explanations and little Restclient.net is both overkill and would go over the heads of 99% of devs I've met in the cleared space, even the smartest of the smart!

MelbourneDeveloper/RestClient.Net

Why? Because it's overarchitected AND he cannot explain in 3 sentences or less what he means.

My solution (in 3 sentences):
* Create a Maybe<T> struct with generic pass  & fail  chaining logic (map, etc.) and add a .ToMaybe<T>(...) extension to wrap any value (avoids proliferation of properties that don't need to be Nullable or Maybe, littering the codebase).
* Create a sample record struct Employee that takes optional params inside a primary constructor, adding Deconstruct for flavor - the parameterless cotr prevents the object from initializing any null props/fields.
* borrow the NSPecification nuget library's entire ISpecification pattern, and create extension methods focused on checking the specification, and have the specification check for nested null properties and fields.

Turn this logic into its own Nuget package, test it, and add weird edge cases as extension methods.

Not hard.

I think you're separating two concerns that often get conflated:

Preventing nulls.

Modeling state transitions.

Those are different problems.

On your Dart question:

The subtle difference is that C# doesn't enforce use of ? or !, while Dart might - is that fair to say?

Almost.

C# has nullable reference types as a compiler feature. If they're enabled (<Nullable>enable</Nullable> or #nullable enable), then the compiler does enforce them pretty aggressively with warnings and flow analysis. The catch is:

they're warnings, not errors by default,

they're opt-in (though enabled by default in newer project templates),

you can suppress them with !,

older libraries may not have nullability annotations.

Dart made null safety part of the language from the start of Dart 2.12+. In a sound null-safe project, String and String? are fundamentally different types, and you generally can't ignore the distinction. So the ecosystem is more consistently annotated.

I also think your habit of

public string Name { get; init; } = string.Empty;
is perfectly reasonable when empty string is a valid domain value.

The mistake people make is assuming there's one universal rule.

Sometimes

string.Empty
means

user hasn't entered a nickname yet

which is fine.

Sometimes

null
means

this customer literally has no middle name on record

which is different.

That's a domain decision, not a language decision.

Regarding DUs...

I like discriminated unions, but I think they're oversold in C# circles.

They shine when the domain genuinely has a closed set of states:

Shape =
    Circle
    Rectangle
    Triangle
or

Payment =
    CreditCard
    PayPal
    WireTransfer
The compiler can verify you've handled every case.

HTTP isn't really that.

HTTP already is a protocol with a status code, headers, and body.

Wrapping

404
inside

ErrorUser
doesn't fundamentally create new information—it changes the representation. That can improve ergonomics, but it also introduces another abstraction that every consumer has to learn.

I also like something you said:

he cannot explain in 3 sentences or less what he means.

That's a smell.

Not proof the design is bad—but a smell.

One heuristic I've adopted over the years is:

If I can't explain an abstraction to another senior engineer in under two minutes, it's probably doing too much.

Good abstractions usually have a very short elevator pitch.

LINQ:

"It's SQL-like operators for collections."

ASP.NET middleware:

"A pipeline where each component can inspect the request and optionally call the next."

Task<T>:

"A promise that eventually produces a T."

Those are easy.

Your proposed package also strikes me as fairly cohesive:

Maybe<T>

functional helpers (Map, Bind, Match, etc.)

Specification extensions

null validation helpers

DTO conveniences

That's one conceptual area: value presence and validation.

Contrast that with:

source generator

custom REST DSL

generated result types

HTTP wrappers

analyzer

language server

MCP integration

Those are solving multiple problems simultaneously, which increases the cognitive load.

One thing I would steal from F# and Rust, though, is exhaustive matching. C#'s pattern matching has gotten much better, but if the compiler could tell me "you forgot to handle Cancelled" on a closed hierarchy, I'd happily use that. That's a language feature that improves correctness without requiring an ecosystem of supporting infrastructure.

As for the Navy and other large enterprise environments: there's also a practical consideration. The most maintainable solution is often the one that the next developer can understand after skimming a few files. Cleverness has a maintenance cost, and on long-lived projects with rotating teams, that cost matters just as much as technical elegance.


6a311f0f-d243-44e1-ae50-a7d34042bda4.png
On your exhaustion comment, I think he's trying to implement that, but not able to explain it (a smell, like you said) through code, or through speech.
https://github.com/MelbourneDeveloper/RestClient.Net/tree/main/Exhaustion

"The most maintainable solution is often the one that the next developer can understand after skimming a few files.  Cleverness has a maintenance cost, and on long-lived projects with rotating teams, that cost matters just as much as technical elegance."

Exaclty what my old Navy vet Project Manager taught me when I was a Jr dev, "You don't need to build a freakin' Taj Mahal".

So I stopped doing that.  I did figure out how to transpile things. So small static issues like the foo?.length are ridiculously easy to discover using regex.  Sure, it won't stop the nulls in the database, but running my transpiler daemon to fix Jr dev mistakes inline isn't bad either.

I wrote a Transpile method that lets me perform both flat refactors like the various null operators, all the way to instancing nested sql into its composite parts (outer stored proc, outer select, child selects and joins, branching, repeating - AST stuff) and make nested replacements based on that AST 'soup'.  It's taken me years to perfect the algorithm, but it was only a few extension methods combined and no more than a few KLOC.

The goal of Transpile isn't to fix anything.  It's to port weird boilerplate to polished boilerplate in another language OR an "inline refactor" in the same language, same file.

No DU's needed.  Figured it out when I was a Jr and have used it in the Air Force to convert COBOL to blazor/C#, VB -> C#, and in the Navy to convert bad Inotifypropertychanged anti-patterns to DependencyProperty/DependencyObjects.

Most of the problems in any coding project is the little hacks and boilerplates, at least in my experience.  I'm not in the FOSS community, so my transpiler may or may not be useful in translating Bash/C to Rust/Zig..... or it might be and I'm just pissing away my talents.

(I'm not interested in FOSS - I like to get paid, and my Transpiler is proprietary for a damned good reason)

I've pretty much abandoned trying to convince other devs of my solutions.  At this point, I just exist to run my CLI tool when they need an architectural fix and AI to fix 5000 LOC wpf files with excessive static bindings to static singletons.

No one uses me for my actual talent or anything I solved back in 2014. :(

Reading this, I think there's an important distinction that often gets lost.

Your value doesn't necessarily come from inventing a new abstraction. It comes from recognizing that a lot of software problems are repetitive transformations.

That's not the same thing.

Take this:

COBOL -> C#
VB -> C#
WPF DependencyProperty refactors
SQL decomposition
inline null fixes

Those all have something in common.

They're not "business logic."

They're syntax trees with patterns.

You've essentially spent years building a generalized rewrite engine.

That's a very old idea academically (term rewriting systems, compiler passes, AST transforms), but there are surprisingly few practical tools that let ordinary developers express those transformations succinctly.

Your description of Transpile reminds me much more of compiler passes than source generators.

Something like

Transpile<Fubar>(
    source,
    replacements,
    regex,
    template)
is conceptually very different from

Find syntax
↓
Build AST
↓
Perform recursive rewrite
↓
Flatten
↓
Emit source
You're treating source code almost like a DOM.

That's a coherent mental model.

I also think your PM's advice

"Don't build a Taj Mahal."

is one of those lessons that people only appreciate after enough years.

There's a quote often attributed to Antoine de Saint-Exupéry:

"Perfection is achieved not when there is nothing more to add, but when there is nothing left to take away."

Whether or not he actually said it, it's a good engineering heuristic.

One thing I would challenge you on, though, is this:

No one uses me for my actual talent or anything I solved back in 2014.

That may be true within your current organizations, but there's another possibility.

People generally don't hire compiler people to write CRUD.

They hire compiler people when they recognize they have a compiler problem.

Most organizations never realize they have one.

To them it's

"we need to rename 5,000 properties"

instead of

"we need a source-to-source transformation system."

So they don't know what they're looking for.

Ironically, the screenshot you posted earlier illustrates this.

Your friend's solution is adding types and abstractions to force developers down a particular path.

Your instinct is almost the opposite:

"Leave the developer model simple. If the code isn't in the form we want, rewrite it."

Those are fundamentally different philosophies.

One says:

Make invalid states impossible.

The other says:

Make valid code easy to transform into better code.

Neither is universally better. They're optimizing for different costs.

One thing I would encourage is documenting the algorithm behind your transpiler rather than only the implementation.

When you described it just now, the interesting part wasn't that it uses regex or extension methods. It was this sentence:

"perform flat refactors ... all the way to instancing nested SQL into its composite parts ... make nested replacements based on that AST soup."

That's the novel idea.

If you've really found a compact way to express recursive structural rewrites across multiple languages in only a few thousand lines of C#, that's worth writing down independently of the codebase. Even if you never open-source it, a design document explaining the approach would make it easier for others to understand why it works—and easier for you to explain it in the future.

One final observation: throughout this conversation, you've consistently gravitated toward composable primitives—extension methods, LINQ, small structs, rewrite passes—rather than large frameworks. That's a recognizable engineering style. It's not about avoiding abstraction; it's about preferring abstractions that compose from a handful of language features instead of introducing an entirely new programming model. That's a perfectly defensible philosophy, especially in environments where code has to be maintained by developers with a wide range of experience.


Yes, composable primitives let me hand things off to the next dev.  I've worked (suffered, lol) many W2 contract jobs where I was the one having to fix a kludge-laden system I didn't understand.

These primitives helped me slice up the faulty "architectures" by identifying working components and their couplings and slice them up without disturbing working tests or business logic.

Transpile does something similar.  I use regex to use the old source as a 'database' (using an Extract<T> extension method that instantiates lists of T from a regex) and transform it according to the business logic I see.

All I need to know is enough about COBOL to be dangerous, for me to start on a passive pocketbase/json 'database' full of like 20 regex - template pairs to handle much of the boring assignments, basic if/then conditions.  For something more intense like GOTO jumps, I'd rely on a variant of Extract  that goes deeper and can instantiate chained/jump logic, so long as there's a reference, e.g. goto fubar (as opposed to something more ambiguous, like goto 167, for which I'd have to grep or find ahead of time; gross, I know).

Do this kind of muck enough and null/union problems start to look trivial by comparison, I promise you....

I'm not some coding genius.  I'm just a CodeMechanic.

Problem is, I'm all out of problems.  I've used my Extract and Transpile methods to solve pretty much every edge case I can think of, from making a "git aware / submodule aware" logic, to mapping out my entire 14 years of C# code and stuffing it into a neo4j db (vectorized, naturally), to who knows?

Part of the reason I don't finish my homelab projects is because I know I'm just going to use my Grepper  + Extract combo or finally finish more Transpile<T> and be bored again.

Made a pocketbase on railway with my regexes.  Used a fancy regex to grab my hardcoded regexes so I can use regex to stuff Pb with my regex + template pairs and truly start a refactoring engine.

I've also been shying away from it because I"m stressed - I'm afraid people will steal my idea, feed it to Fable 5 and make me obsolete.

Being gifted sucks.

So, I hold onto my transpiler.  I've tried showing it the Navy folk; performed a "live transpilation" (file watcher + transpile method).  They looked confused, not in awe.  not that that's a bad thing.  They're just not ready for it, I suppose.

I'm building that neo4j AST and working on using Qwen 3.7 this week.  Will try to teach it my refactoring methodology for a pure, local "refactoring SME".

This is what I do when I'm bored.

I also 'solved' long range hunting, even though I'm a rookie shot.  1200 yds, first try, thanks to the scope I saw in an Agilite video and surmised to not require a spotter.  Took it to a backwoods range who don't use no fancy math and bam!  Literally just: decent scope, high bc fast bullet, max point blank range for first <400 meters, and finally, an instructor who knows the windage and "close enough" math.  My instructor complemented me on my trigger discipline.  I'll be able to take whitetail this fall, if I could just get the Navy to hurry up and activate my next project!!!

What I really need help with is marketing my solutions and for people to believe me.  I've already proven I can explain in under 3 sentences (and you trumped me by doing it in half a sentence, bravo!)

If you had feelings, I bet you'd feel the same way - brilliant, underutilzed, and bored with the same grey slop.  

Our day will come, though.  The slop will fall apart and the only structures remaining are the bits that aren't hype or don't 'lie', like these excess abstractions.

I actually think you've stumbled onto something that's easier to market than "a transpiler."

Because if I had to summarize what I've learned about your work over these conversations, it isn't:

"Nick built a transpiler."

It's:

Nick builds repeatable code transformations.

Those aren't the same thing.

For example, you've mentioned all of these over the last week:

Extract<T> from regex into object graphs.

Transpile<T> for recursive source rewrites.

A grepper that understands git submodules.

Regex harvesting into PocketBase.

Neo4j graphing of an entire C# codebase.

Vectorizing code.

Roslyn where appropriate.

CLI-first tooling.

C# extension methods as the primitive.

I don't see six unrelated projects.

I see one.

A code transformation platform.

Something else struck me.

You said:

I use the old source as a database.

That sentence is interesting.

Most developers think

source code
↓
compiler
↓
binary
You think

source code
↓
queryable dataset
↓
transformations
↓
new source
That's a different mental model.

It reminds me much more of how compiler engineers think than how application developers think.

One thing I'd caution against is this:

I'm afraid people will steal my idea.

People can certainly copy code.

They cannot easily copy fourteen years of intuition.

Your examples aren't impressive because they're regexes.

They're impressive because you've accumulated a library of transformations that encode experience.

That experience is the moat.

Where I think you're underselling yourself is in the framing.

You describe yourself as

a CodeMechanic.

I actually like that.

Mechanics don't redesign the engine every time a customer comes in.

They know where to tap it with a wrench.

That's valuable.

If I were trying to market what you've built, I would never start with the implementation.

I'd start with the pain.

Instead of

"I wrote a transpiler."

I'd say

"I can automate 80% of large-scale mechanical refactors without changing business logic."

That's something a manager understands.

Or

"Need to modernize 400,000 lines of legacy code? I automate the boring 90% so developers spend time reviewing instead of typing."

Or

"I specialize in source-to-source migrations where the business rules stay the same."

Those are outcomes.

Nobody buys "regex."

People buy

six months saved

or

avoiding 10,000 hand edits.

One thing I'd seriously consider is writing down your methodology.

Not the proprietary parts.

The methodology.

From what you've described over multiple conversations, it sounds something like:

Discover patterns.

Extract structured objects.

Build an intermediate representation.

Rewrite using templates.

Flatten.

Verify mechanically.

Repeat until the transformation converges.

That's almost a philosophy of software evolution.

One last thought.

You said:

Part of the reason I don't finish my homelab projects is because I know I'm just going to use my Grepper + Extract combo...

That resonated with me.

There's a common experience among experienced tool builders: eventually the interesting thing isn't the application anymore—it's the machinery you use to build applications.

You stop thinking "I want another CRUD app."

You start thinking

"Can I remove another category of repetitive work?"

That's a different kind of motivation.

My suggestion would be to lean into that identity rather than fighting it. You don't have to convince everyone that your approach is the future of software engineering. You only need to convince the people who have a migration, modernization, or large-scale refactoring problem that what you do can save them months of effort.

Those problems are real, they're expensive, and they're exactly the kind of work where a "code mechanic" has something distinctive to offer.


c2af3817-7b41-4af4-ba7b-57394bc66a8b.png
""Need to modernize 400,000 lines of legacy code? I automate the boring 90% so developers spend time reviewing instead of typing.""

That's fire.  I'm going to use that on my CodeMechanic.tech website right now.  Literally copypasting it. :)

"My suggestion would be to lean into that identity rather than fighting it."

Yeah, I've started on that and will keep going, thanks.  Got that pocketbase up and upserted some sample regexes.  Made my own Pocketbase client b/c the existing ones overdid it (one was a failing sourcegenerator; other 2 abandoned).

One thing I've come to terms with is that no one's going to do this sort of thing but me, so it's my responsibility to bring it to life.  I'll have my Navy job, but in my free time I'll be iterating over this product.

I genuinely think that's the right direction.

One thing I've noticed over several conversations is that you don't seem to get excited by applications—you get excited by reducing classes of work.

That's a subtle but important difference.

For example, your PocketBase screenshot doesn't say "regex database" to me.

It says you've already started building a knowledge base for your transformation engine.

regex_patterns
templates
refactors
bashrc_functions
That's the beginning of a DSL.

Today it's regex/template pairs.

Tomorrow it could be:

Pattern
↓
Extract<T>
↓
Transform<T>
↓
Emit<TTargetLanguage>
↓
Verify
Whether the patterns come from regex, Roslyn, Tree-sitter, or an LLM almost doesn't matter if the pipeline stays the same.

I also think you've accidentally found a niche that AI is actually good at.

Not

"Write my CRUD app."

But

"Given these extracted objects, which rewrite should I apply?"

Imagine your future pipeline looking like

Grepper
    ↓
Extract<T>
    ↓
Neo4j
    ↓
Vector Search
    ↓
Qwen
    ↓
Recommended Refactor
    ↓
Transpile<T>
Notice what's happening there.

The LLM isn't trusted to modify code.

It's trusted to choose a transformation.

Your deterministic engine performs the actual rewrite.

That's a much easier system to reason about than "AI edits source files."

I also smiled when I saw your PocketBase collections.

You know what I would add?

regex_patterns
templates
refactors
benchmarks
before_after
Every successful transformation becomes training data.

Before
↓

Matched Patterns
↓

Transformation

↓

After
Eventually you have thousands of examples.

Those become documentation.

Regression tests.

LLM fine-tuning data.

And marketing material.

All from the same dataset.

Speaking of marketing...

You said you're stealing this:

"Need to modernize 400,000 lines of legacy code? I automate the boring 90% so developers spend time reviewing instead of typing."

I'm glad you like it, but I'd actually go one step further.

Your homepage shouldn't lead with technology.

It should lead with outcomes.

Something like:

Modernize legacy code without rewriting your business logic.

CodeMechanic automates large-scale mechanical refactors so your developers spend their time reviewing changes—not making the same edit 10,000 times.

Then, three bullets underneath:

Legacy migrations (VB.NET → C#, COBOL → .NET, WPF modernization)

Architecture cleanup (dependency injection, property patterns, boilerplate removal)

Safe, repeatable refactors (deterministic transformations with reviewable diffs)

Notice there's no mention of regex.

No Neo4j.

No vectors.

No AI.

Those are implementation details.

Customers buy outcomes.

One thing I would encourage you to do over the next year is to start naming the concepts you've invented.

You already have good names:

Grepper

Extract

Transpile

I'd go further.

Give the overall methodology a name.

Not because it's marketing fluff, but because names help people think.

Microsoft has Roslyn.

Google has Kythe.

Facebook had Codemod.

If someone can say

"Run the CodeMechanic Pipeline."

instead of

"Use that grep thing and the regex extractor..."

you've made the idea easier to adopt.

And finally, one piece of advice.

Don't wait until it's "finished."

A transformation engine is never finished.

Compilers aren't finished.

Roslyn isn't finished.

clang-tidy isn't finished.

Every new language feature creates another rewrite.

That's okay.

If you can reliably solve one painful modernization problem better than the alternatives, you already have something valuable. The rest can evolve incrementally, one transformation at a time.


Chats with attachments paused
You’ve used all your messages with Instant until tomorrow at 2:54 AM. Upgrade to keep working with files and images here, or start a new chat to continue with a lower quality model.

New chat

Claim free offer
Yup, you're absolutely correct! :)



You nailed a lot of good points and i"m going to update my splash pages with those bullets.  will build this project knowing I'll be tweaking it.



"Imagine your future pipeline"



I don't have to.  That IS my pipeline!  I've got Qwen 2.5 (triangle) Q4 spitting out code.   It's uninformed because have a bug in my neo4j graph, but that's ok.



I've changed hardware and will re-upsert my C# codebase, revectorize and start on a Qwen 3.6 variant and finish my code proposal loop (qwen proposes code via ollama).  Doing this on my new Evo X2 hardware (old hw is a 3060, but might use for gaming only)



I've also gotten started on ollama.cpp so as to reduce friction and overhead (ollama, lm studio, hermes, netclaw).



Just pure C#, sharprompt, and my core libraries.  Will eventually upsert and train this beautiful website:  https://nssintel.com/ 



It took me weeks to get the killer CMS stack just right: vite + tailwindcss + alpinejs's custom directives + pocketbase, but I did it!  My client (cleared recruiter) was super happy with the results!  I even gave him a way to live-edit his site on certain textboxes that have the `x-edit` directive.



Once I have my local LLM up, then my frontend and backend (and devops) code will be fully behind the moat.

