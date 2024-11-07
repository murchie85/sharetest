#!/usr/bin/perl
use strict;
use warnings;
use LWP::UserAgent;
use HTTP::Request;
use HTTP::Response;
use HTTP::Cookies;
use MIME::Base64;

# Replace 'REDACTED' with your actual username, password, and URI
my $username = 'REDACTED';
my $password = 'REDACTED';
my $uri = 'REDACTED';

# Create UserAgent 1 with its own cookie jar
my $ua1 = LWP::UserAgent->new(
    agent => 'curl/123',
    max_redirect => 0,  # Do not follow redirects automatically
);
$ua1->default_header('Accept' => '*/*');
$ua1->cookie_jar(HTTP::Cookies->new());

# Create UserAgent 2 with a separate cookie jar
my $ua2 = LWP::UserAgent->new(
    agent => 'curl/123',
    max_redirect => 0,
);
$ua2->default_header('Accept' => '*/*');
$ua2->cookie_jar(HTTP::Cookies->new());

# Encode credentials for Basic Authentication
my $auth = encode_base64("$username:$password", '');

# First request with ua1
my $req1 = HTTP::Request->new('GET', $uri);
$req1->header('Authorization' => "Basic $auth");
my $res1 = $ua1->request($req1);

# Get the next URL from the Location header
my $next = $res1->header('Location');

# Second request with ua2
my $req2 = HTTP::Request->new('GET', $next);
$req2->header('Authorization' => "Basic $auth");
my $res2 = $ua2->request($req2);

# Get the next URL from the Location header
my $next2 = $res2->header('Location');

# Third request with ua2
my $req3 = HTTP::Request->new('GET', $next2);
$req3->header('Authorization' => "Basic $auth");
my $res3 = $ua2->request($req3);

# Get the return URL from the Location header
my $returnurl = $res3->header('Location');

# Fourth request with ua1, allowing up to 10 redirects
$ua1->max_redirect(10);
my $req4 = HTTP::Request->new('GET', $returnurl);
$req4->header('Authorization' => "Basic $auth");
my $res4 = $ua1->request($req4);

# Print the final response
print $res4->as_string;


#cpan install LWP::UserAgent HTTP::Request HTTP::Cookies MIME::Base64
