--
-- PostgreSQL database dump
--

-- Dumped from database version 17.5
-- Dumped by pg_dump version 17.5

-- Started on 2025-07-07 19:59:12

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- TOC entry 240 (class 1259 OID 16590)
-- Name: Cart; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Cart" (
    "Id" integer NOT NULL,
    "UserId" integer NOT NULL
);


ALTER TABLE public."Cart" OWNER TO postgres;

--
-- TOC entry 242 (class 1259 OID 16602)
-- Name: CartItems; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."CartItems" (
    "Id" integer NOT NULL,
    "CartId " integer NOT NULL,
    "ComicId" integer NOT NULL,
    "Quantity" integer DEFAULT 1 NOT NULL
);


ALTER TABLE public."CartItems" OWNER TO postgres;

--
-- TOC entry 241 (class 1259 OID 16601)
-- Name: CartItems_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."CartItems_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."CartItems_Id_seq" OWNER TO postgres;

--
-- TOC entry 5053 (class 0 OID 0)
-- Dependencies: 241
-- Name: CartItems_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."CartItems_Id_seq" OWNED BY public."CartItems"."Id";


--
-- TOC entry 239 (class 1259 OID 16589)
-- Name: Cart_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Cart_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Cart_Id_seq" OWNER TO postgres;

--
-- TOC entry 5054 (class 0 OID 0)
-- Dependencies: 239
-- Name: Cart_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Cart_Id_seq" OWNED BY public."Cart"."Id";


--
-- TOC entry 218 (class 1259 OID 16391)
-- Name: Categories; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Categories" (
    "Id" integer NOT NULL
);


ALTER TABLE public."Categories" OWNER TO postgres;

--
-- TOC entry 249 (class 1259 OID 16720)
-- Name: CategoriesTranslations; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."CategoriesTranslations" (
    "Id" integer NOT NULL,
    "LanguageId" integer NOT NULL,
    "Name" character varying(50),
    "CategoryId" integer NOT NULL
);


ALTER TABLE public."CategoriesTranslations" OWNER TO postgres;

--
-- TOC entry 248 (class 1259 OID 16719)
-- Name: CategoriesTranslations_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."CategoriesTranslations_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."CategoriesTranslations_Id_seq" OWNER TO postgres;

--
-- TOC entry 5055 (class 0 OID 0)
-- Dependencies: 248
-- Name: CategoriesTranslations_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."CategoriesTranslations_Id_seq" OWNED BY public."CategoriesTranslations"."Id";


--
-- TOC entry 222 (class 1259 OID 16405)
-- Name: Chapters; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Chapters" (
    "Id" integer NOT NULL,
    "ComicId" integer NOT NULL,
    "Position" integer NOT NULL,
    "Original_Language" integer NOT NULL,
    "Status" integer NOT NULL,
    "Release_Date" date
);


ALTER TABLE public."Chapters" OWNER TO postgres;

--
-- TOC entry 245 (class 1259 OID 16691)
-- Name: ColorType; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."ColorType" (
    "Id" integer NOT NULL
);


ALTER TABLE public."ColorType" OWNER TO postgres;

--
-- TOC entry 254 (class 1259 OID 16816)
-- Name: ColorTypeTranslations; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."ColorTypeTranslations" (
    "Id" integer NOT NULL,
    "ColorTypeId" integer,
    "LanguageId" integer,
    "Name" character varying(20)
);


ALTER TABLE public."ColorTypeTranslations" OWNER TO postgres;

--
-- TOC entry 253 (class 1259 OID 16815)
-- Name: ColorTypeTranslations_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."ColorTypeTranslations_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."ColorTypeTranslations_Id_seq" OWNER TO postgres;

--
-- TOC entry 5056 (class 0 OID 0)
-- Dependencies: 253
-- Name: ColorTypeTranslations_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."ColorTypeTranslations_Id_seq" OWNED BY public."ColorTypeTranslations"."Id";


--
-- TOC entry 244 (class 1259 OID 16690)
-- Name: ColorType_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."ColorType_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."ColorType_Id_seq" OWNER TO postgres;

--
-- TOC entry 5057 (class 0 OID 0)
-- Dependencies: 244
-- Name: ColorType_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."ColorType_Id_seq" OWNED BY public."ColorType"."Id";


--
-- TOC entry 243 (class 1259 OID 16661)
-- Name: ComicCategory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."ComicCategory" (
    "ComicId" integer NOT NULL,
    "CategoryId" integer NOT NULL
);


ALTER TABLE public."ComicCategory" OWNER TO postgres;

--
-- TOC entry 220 (class 1259 OID 16398)
-- Name: Comics; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Comics" (
    "Id" integer NOT NULL,
    "Publish_Date" date,
    "Total_Pages" integer,
    "Available_Storage" integer,
    "Color" integer DEFAULT 1,
    "Cover" integer DEFAULT 1,
    "Status" integer
);


ALTER TABLE public."Comics" OWNER TO postgres;

--
-- TOC entry 227 (class 1259 OID 16442)
-- Name: ComicsMembers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."ComicsMembers" (
    "ComicId" integer NOT NULL,
    "TeamMemberId" integer NOT NULL
);


ALTER TABLE public."ComicsMembers" OWNER TO postgres;

--
-- TOC entry 256 (class 1259 OID 16833)
-- Name: ComicsTranslations; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."ComicsTranslations" (
    "Id" integer NOT NULL,
    "ComicId" integer,
    "LanguageId" integer,
    "Name" character varying(100),
    "Description" text,
    "Amazon" character varying(100),
    "Catarse" character varying(100),
    "Price" numeric DEFAULT 0,
    "Discount" integer DEFAULT 0,
    "Pictures" character varying[]
);


ALTER TABLE public."ComicsTranslations" OWNER TO postgres;

--
-- TOC entry 255 (class 1259 OID 16832)
-- Name: ComicsTranslations_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."ComicsTranslations_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."ComicsTranslations_Id_seq" OWNER TO postgres;

--
-- TOC entry 5058 (class 0 OID 0)
-- Dependencies: 255
-- Name: ComicsTranslations_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."ComicsTranslations_Id_seq" OWNED BY public."ComicsTranslations"."Id";


--
-- TOC entry 257 (class 1259 OID 16852)
-- Name: CoverType; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."CoverType" (
    "Id" integer NOT NULL
);


ALTER TABLE public."CoverType" OWNER TO postgres;

--
-- TOC entry 259 (class 1259 OID 16864)
-- Name: CoverTypeTranslations; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."CoverTypeTranslations" (
    "Id" integer NOT NULL,
    "CoverTypeId" integer NOT NULL,
    "LanguageId" integer NOT NULL,
    "Name" character varying(20)
);


ALTER TABLE public."CoverTypeTranslations" OWNER TO postgres;

--
-- TOC entry 258 (class 1259 OID 16863)
-- Name: CoverTypeTranslations_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."CoverTypeTranslations_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."CoverTypeTranslations_Id_seq" OWNER TO postgres;

--
-- TOC entry 5059 (class 0 OID 0)
-- Dependencies: 258
-- Name: CoverTypeTranslations_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."CoverTypeTranslations_Id_seq" OWNED BY public."CoverTypeTranslations"."Id";


--
-- TOC entry 261 (class 1259 OID 16887)
-- Name: CoverType_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."CoverType_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."CoverType_Id_seq" OWNER TO postgres;

--
-- TOC entry 5060 (class 0 OID 0)
-- Dependencies: 261
-- Name: CoverType_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."CoverType_Id_seq" OWNED BY public."CoverType"."Id";


--
-- TOC entry 229 (class 1259 OID 16459)
-- Name: FanArts; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."FanArts" (
    "Id" integer NOT NULL,
    "Social" character varying(50) NOT NULL,
    "Picture" text NOT NULL
);


ALTER TABLE public."FanArts" OWNER TO postgres;

--
-- TOC entry 247 (class 1259 OID 16713)
-- Name: Languages; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Languages" (
    "Id" integer NOT NULL,
    "Language" character varying(5)
);


ALTER TABLE public."Languages" OWNER TO postgres;

--
-- TOC entry 246 (class 1259 OID 16712)
-- Name: Languages_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Languages_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Languages_Id_seq" OWNER TO postgres;

--
-- TOC entry 5061 (class 0 OID 0)
-- Dependencies: 246
-- Name: Languages_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Languages_Id_seq" OWNED BY public."Languages"."Id";


--
-- TOC entry 266 (class 1259 OID 16953)
-- Name: Likes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Likes" (
    "Id" integer NOT NULL,
    "UserId" integer NOT NULL,
    "ComicId" integer NOT NULL
);


ALTER TABLE public."Likes" OWNER TO postgres;

--
-- TOC entry 265 (class 1259 OID 16952)
-- Name: Likes_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Likes_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Likes_Id_seq" OWNER TO postgres;

--
-- TOC entry 5062 (class 0 OID 0)
-- Dependencies: 265
-- Name: Likes_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Likes_Id_seq" OWNED BY public."Likes"."Id";


--
-- TOC entry 268 (class 1259 OID 16970)
-- Name: Loves; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Loves" (
    "Id" integer NOT NULL,
    "UserId" integer NOT NULL,
    "ComicId" integer
);


ALTER TABLE public."Loves" OWNER TO postgres;

--
-- TOC entry 267 (class 1259 OID 16969)
-- Name: Loves_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Loves_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Loves_Id_seq" OWNER TO postgres;

--
-- TOC entry 5063 (class 0 OID 0)
-- Dependencies: 267
-- Name: Loves_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Loves_Id_seq" OWNED BY public."Loves"."Id";


--
-- TOC entry 231 (class 1259 OID 16468)
-- Name: Orders; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Orders" (
    "Id" integer NOT NULL
);


ALTER TABLE public."Orders" OWNER TO postgres;

--
-- TOC entry 270 (class 1259 OID 16992)
-- Name: Pages; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Pages" (
    "Id" integer NOT NULL,
    "ChapterId" integer NOT NULL,
    "Picture" character varying(150) NOT NULL,
    "Position" integer
);


ALTER TABLE public."Pages" OWNER TO postgres;

--
-- TOC entry 269 (class 1259 OID 16991)
-- Name: Pages_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Pages_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Pages_Id_seq" OWNER TO postgres;

--
-- TOC entry 5064 (class 0 OID 0)
-- Dependencies: 269
-- Name: Pages_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Pages_Id_seq" OWNED BY public."Pages"."Id";


--
-- TOC entry 233 (class 1259 OID 16475)
-- Name: Permissions; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Permissions" (
    "Id" integer NOT NULL,
    "Name" character varying(30) NOT NULL
);


ALTER TABLE public."Permissions" OWNER TO postgres;

--
-- TOC entry 237 (class 1259 OID 16496)
-- Name: RefreshTokens; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."RefreshTokens" (
    "Id" integer NOT NULL,
    "Token" text NOT NULL,
    "Expires" date NOT NULL,
    "UserId" integer NOT NULL
);


ALTER TABLE public."RefreshTokens" OWNER TO postgres;

--
-- TOC entry 224 (class 1259 OID 16419)
-- Name: Roles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Roles" (
    "Id" integer NOT NULL
);


ALTER TABLE public."Roles" OWNER TO postgres;

--
-- TOC entry 251 (class 1259 OID 16739)
-- Name: RolesTranslations; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."RolesTranslations" (
    "RoleId" integer NOT NULL,
    "LanguageId" integer NOT NULL,
    "Name" character varying(40),
    "Id" integer NOT NULL
);


ALTER TABLE public."RolesTranslations" OWNER TO postgres;

--
-- TOC entry 252 (class 1259 OID 16755)
-- Name: RolesTranslations_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."RolesTranslations_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."RolesTranslations_Id_seq" OWNER TO postgres;

--
-- TOC entry 5065 (class 0 OID 0)
-- Dependencies: 252
-- Name: RolesTranslations_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."RolesTranslations_Id_seq" OWNED BY public."RolesTranslations"."Id";


--
-- TOC entry 250 (class 1259 OID 16738)
-- Name: RolesTranslations_LanguageId_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."RolesTranslations_LanguageId_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."RolesTranslations_LanguageId_seq" OWNER TO postgres;

--
-- TOC entry 5066 (class 0 OID 0)
-- Dependencies: 250
-- Name: RolesTranslations_LanguageId_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."RolesTranslations_LanguageId_seq" OWNED BY public."RolesTranslations"."LanguageId";


--
-- TOC entry 260 (class 1259 OID 16881)
-- Name: Status; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Status" (
    "Id" integer NOT NULL
);


ALTER TABLE public."Status" OWNER TO postgres;

--
-- TOC entry 264 (class 1259 OID 16912)
-- Name: StatusTranslations; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."StatusTranslations" (
    "Id" integer NOT NULL,
    "LanguageId" integer NOT NULL,
    "StatusId" integer NOT NULL,
    "Name" character varying(40)
);


ALTER TABLE public."StatusTranslations" OWNER TO postgres;

--
-- TOC entry 263 (class 1259 OID 16911)
-- Name: StatusTranslations_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."StatusTranslations_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."StatusTranslations_Id_seq" OWNER TO postgres;

--
-- TOC entry 5067 (class 0 OID 0)
-- Dependencies: 263
-- Name: StatusTranslations_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."StatusTranslations_Id_seq" OWNED BY public."StatusTranslations"."Id";


--
-- TOC entry 262 (class 1259 OID 16904)
-- Name: Status_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."Status_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."Status_Id_seq" OWNER TO postgres;

--
-- TOC entry 5068 (class 0 OID 0)
-- Dependencies: 262
-- Name: Status_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."Status_Id_seq" OWNED BY public."Status"."Id";


--
-- TOC entry 238 (class 1259 OID 16510)
-- Name: TeamMemberRole; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."TeamMemberRole" (
    "TeamMemberId" integer NOT NULL,
    "RolesId" integer NOT NULL
);


ALTER TABLE public."TeamMemberRole" OWNER TO postgres;

--
-- TOC entry 226 (class 1259 OID 16426)
-- Name: TeamMembers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."TeamMembers" (
    "Id" integer NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Social" character varying(50) NOT NULL,
    "Picture" text,
    "Official_Member" boolean
);


ALTER TABLE public."TeamMembers" OWNER TO postgres;

--
-- TOC entry 235 (class 1259 OID 16482)
-- Name: Users; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public."Users" (
    "Id" integer NOT NULL,
    "UserName" character varying(50) NOT NULL,
    "Email" character varying(50) NOT NULL,
    "Password" character varying(100) NOT NULL,
    "Picture" text,
    "PermissionId" integer DEFAULT 2 NOT NULL
);


ALTER TABLE public."Users" OWNER TO postgres;

--
-- TOC entry 217 (class 1259 OID 16390)
-- Name: categories_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."categories_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."categories_Id_seq" OWNER TO postgres;

--
-- TOC entry 5069 (class 0 OID 0)
-- Dependencies: 217
-- Name: categories_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."categories_Id_seq" OWNED BY public."Categories"."Id";


--
-- TOC entry 221 (class 1259 OID 16404)
-- Name: chapters_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."chapters_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."chapters_Id_seq" OWNER TO postgres;

--
-- TOC entry 5070 (class 0 OID 0)
-- Dependencies: 221
-- Name: chapters_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."chapters_Id_seq" OWNED BY public."Chapters"."Id";


--
-- TOC entry 219 (class 1259 OID 16397)
-- Name: comics_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."comics_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."comics_Id_seq" OWNER TO postgres;

--
-- TOC entry 5071 (class 0 OID 0)
-- Dependencies: 219
-- Name: comics_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."comics_Id_seq" OWNED BY public."Comics"."Id";


--
-- TOC entry 228 (class 1259 OID 16458)
-- Name: fanarts_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."fanarts_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."fanarts_Id_seq" OWNER TO postgres;

--
-- TOC entry 5072 (class 0 OID 0)
-- Dependencies: 228
-- Name: fanarts_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."fanarts_Id_seq" OWNED BY public."FanArts"."Id";


--
-- TOC entry 230 (class 1259 OID 16467)
-- Name: orders_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."orders_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."orders_Id_seq" OWNER TO postgres;

--
-- TOC entry 5073 (class 0 OID 0)
-- Dependencies: 230
-- Name: orders_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."orders_Id_seq" OWNED BY public."Orders"."Id";


--
-- TOC entry 232 (class 1259 OID 16474)
-- Name: permissions_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."permissions_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."permissions_Id_seq" OWNER TO postgres;

--
-- TOC entry 5074 (class 0 OID 0)
-- Dependencies: 232
-- Name: permissions_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."permissions_Id_seq" OWNED BY public."Permissions"."Id";


--
-- TOC entry 236 (class 1259 OID 16495)
-- Name: refreshtokens_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."refreshtokens_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."refreshtokens_Id_seq" OWNER TO postgres;

--
-- TOC entry 5075 (class 0 OID 0)
-- Dependencies: 236
-- Name: refreshtokens_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."refreshtokens_Id_seq" OWNED BY public."RefreshTokens"."Id";


--
-- TOC entry 223 (class 1259 OID 16418)
-- Name: roles_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."roles_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."roles_Id_seq" OWNER TO postgres;

--
-- TOC entry 5076 (class 0 OID 0)
-- Dependencies: 223
-- Name: roles_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."roles_Id_seq" OWNED BY public."Roles"."Id";


--
-- TOC entry 225 (class 1259 OID 16425)
-- Name: teammembers_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."teammembers_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."teammembers_Id_seq" OWNER TO postgres;

--
-- TOC entry 5077 (class 0 OID 0)
-- Dependencies: 225
-- Name: teammembers_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."teammembers_Id_seq" OWNED BY public."TeamMembers"."Id";


--
-- TOC entry 234 (class 1259 OID 16481)
-- Name: users_Id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public."users_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public."users_Id_seq" OWNER TO postgres;

--
-- TOC entry 5078 (class 0 OID 0)
-- Dependencies: 234
-- Name: users_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public."users_Id_seq" OWNED BY public."Users"."Id";


--
-- TOC entry 4787 (class 2604 OID 16593)
-- Name: Cart Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Cart" ALTER COLUMN "Id" SET DEFAULT nextval('public."Cart_Id_seq"'::regclass);


--
-- TOC entry 4788 (class 2604 OID 16605)
-- Name: CartItems Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CartItems" ALTER COLUMN "Id" SET DEFAULT nextval('public."CartItems_Id_seq"'::regclass);


--
-- TOC entry 4774 (class 2604 OID 16394)
-- Name: Categories Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Categories" ALTER COLUMN "Id" SET DEFAULT nextval('public."categories_Id_seq"'::regclass);


--
-- TOC entry 4792 (class 2604 OID 16723)
-- Name: CategoriesTranslations Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CategoriesTranslations" ALTER COLUMN "Id" SET DEFAULT nextval('public."CategoriesTranslations_Id_seq"'::regclass);


--
-- TOC entry 4778 (class 2604 OID 16408)
-- Name: Chapters Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Chapters" ALTER COLUMN "Id" SET DEFAULT nextval('public."chapters_Id_seq"'::regclass);


--
-- TOC entry 4790 (class 2604 OID 16694)
-- Name: ColorType Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ColorType" ALTER COLUMN "Id" SET DEFAULT nextval('public."ColorType_Id_seq"'::regclass);


--
-- TOC entry 4794 (class 2604 OID 16819)
-- Name: ColorTypeTranslations Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ColorTypeTranslations" ALTER COLUMN "Id" SET DEFAULT nextval('public."ColorTypeTranslations_Id_seq"'::regclass);


--
-- TOC entry 4775 (class 2604 OID 16401)
-- Name: Comics Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Comics" ALTER COLUMN "Id" SET DEFAULT nextval('public."comics_Id_seq"'::regclass);


--
-- TOC entry 4795 (class 2604 OID 16836)
-- Name: ComicsTranslations Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ComicsTranslations" ALTER COLUMN "Id" SET DEFAULT nextval('public."ComicsTranslations_Id_seq"'::regclass);


--
-- TOC entry 4798 (class 2604 OID 16888)
-- Name: CoverType Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CoverType" ALTER COLUMN "Id" SET DEFAULT nextval('public."CoverType_Id_seq"'::regclass);


--
-- TOC entry 4799 (class 2604 OID 16867)
-- Name: CoverTypeTranslations Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CoverTypeTranslations" ALTER COLUMN "Id" SET DEFAULT nextval('public."CoverTypeTranslations_Id_seq"'::regclass);


--
-- TOC entry 4781 (class 2604 OID 16462)
-- Name: FanArts Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."FanArts" ALTER COLUMN "Id" SET DEFAULT nextval('public."fanarts_Id_seq"'::regclass);


--
-- TOC entry 4791 (class 2604 OID 16716)
-- Name: Languages Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Languages" ALTER COLUMN "Id" SET DEFAULT nextval('public."Languages_Id_seq"'::regclass);


--
-- TOC entry 4802 (class 2604 OID 16956)
-- Name: Likes Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Likes" ALTER COLUMN "Id" SET DEFAULT nextval('public."Likes_Id_seq"'::regclass);


--
-- TOC entry 4803 (class 2604 OID 16973)
-- Name: Loves Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Loves" ALTER COLUMN "Id" SET DEFAULT nextval('public."Loves_Id_seq"'::regclass);


--
-- TOC entry 4782 (class 2604 OID 16471)
-- Name: Orders Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Orders" ALTER COLUMN "Id" SET DEFAULT nextval('public."orders_Id_seq"'::regclass);


--
-- TOC entry 4804 (class 2604 OID 16995)
-- Name: Pages Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Pages" ALTER COLUMN "Id" SET DEFAULT nextval('public."Pages_Id_seq"'::regclass);


--
-- TOC entry 4783 (class 2604 OID 16478)
-- Name: Permissions Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Permissions" ALTER COLUMN "Id" SET DEFAULT nextval('public."permissions_Id_seq"'::regclass);


--
-- TOC entry 4786 (class 2604 OID 16499)
-- Name: RefreshTokens Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RefreshTokens" ALTER COLUMN "Id" SET DEFAULT nextval('public."refreshtokens_Id_seq"'::regclass);


--
-- TOC entry 4779 (class 2604 OID 16422)
-- Name: Roles Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Roles" ALTER COLUMN "Id" SET DEFAULT nextval('public."roles_Id_seq"'::regclass);


--
-- TOC entry 4793 (class 2604 OID 16756)
-- Name: RolesTranslations Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RolesTranslations" ALTER COLUMN "Id" SET DEFAULT nextval('public."RolesTranslations_Id_seq"'::regclass);


--
-- TOC entry 4800 (class 2604 OID 16905)
-- Name: Status Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Status" ALTER COLUMN "Id" SET DEFAULT nextval('public."Status_Id_seq"'::regclass);


--
-- TOC entry 4801 (class 2604 OID 16915)
-- Name: StatusTranslations Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."StatusTranslations" ALTER COLUMN "Id" SET DEFAULT nextval('public."StatusTranslations_Id_seq"'::regclass);


--
-- TOC entry 4780 (class 2604 OID 16429)
-- Name: TeamMembers Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TeamMembers" ALTER COLUMN "Id" SET DEFAULT nextval('public."teammembers_Id_seq"'::regclass);


--
-- TOC entry 4784 (class 2604 OID 16485)
-- Name: Users Id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Users" ALTER COLUMN "Id" SET DEFAULT nextval('public."users_Id_seq"'::regclass);


--
-- TOC entry 4840 (class 2606 OID 16608)
-- Name: CartItems CartItems_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CartItems"
    ADD CONSTRAINT "CartItems_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4838 (class 2606 OID 16595)
-- Name: Cart Cart_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Cart"
    ADD CONSTRAINT "Cart_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4848 (class 2606 OID 16725)
-- Name: CategoriesTranslations CategoriesTranslations_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CategoriesTranslations"
    ADD CONSTRAINT "CategoriesTranslations_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4852 (class 2606 OID 16821)
-- Name: ColorTypeTranslations ColorTypeTranslations_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ColorTypeTranslations"
    ADD CONSTRAINT "ColorTypeTranslations_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4844 (class 2606 OID 16696)
-- Name: ColorType ColorType_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ColorType"
    ADD CONSTRAINT "ColorType_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4842 (class 2606 OID 16737)
-- Name: ComicCategory ComicCategory_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ComicCategory"
    ADD CONSTRAINT "ComicCategory_pkey" PRIMARY KEY ("ComicId", "CategoryId");


--
-- TOC entry 4854 (class 2606 OID 16838)
-- Name: ComicsTranslations ComicsTranslations_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ComicsTranslations"
    ADD CONSTRAINT "ComicsTranslations_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4858 (class 2606 OID 16869)
-- Name: CoverTypeTranslations CoverTypeTranslations_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CoverTypeTranslations"
    ADD CONSTRAINT "CoverTypeTranslations_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4856 (class 2606 OID 16893)
-- Name: CoverType CoverType_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CoverType"
    ADD CONSTRAINT "CoverType_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4846 (class 2606 OID 16718)
-- Name: Languages Languages_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Languages"
    ADD CONSTRAINT "Languages_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4864 (class 2606 OID 16958)
-- Name: Likes Likes_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Likes"
    ADD CONSTRAINT "Likes_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4866 (class 2606 OID 16975)
-- Name: Loves Loves_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Loves"
    ADD CONSTRAINT "Loves_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4814 (class 2606 OID 16544)
-- Name: TeamMembers MemberSocial; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TeamMembers"
    ADD CONSTRAINT "MemberSocial" UNIQUE ("Social");


--
-- TOC entry 4818 (class 2606 OID 16526)
-- Name: ComicsMembers PK_TeamMemberComic; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ComicsMembers"
    ADD CONSTRAINT "PK_TeamMemberComic" PRIMARY KEY ("ComicId", "TeamMemberId");


--
-- TOC entry 4836 (class 2606 OID 16784)
-- Name: TeamMemberRole PK_TeamMemberRole; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TeamMemberRole"
    ADD CONSTRAINT "PK_TeamMemberRole" PRIMARY KEY ("TeamMemberId", "RolesId");


--
-- TOC entry 4868 (class 2606 OID 16997)
-- Name: Pages Pages_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Pages"
    ADD CONSTRAINT "Pages_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4824 (class 2606 OID 16536)
-- Name: Permissions PermissionName; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Permissions"
    ADD CONSTRAINT "PermissionName" UNIQUE ("Name");


--
-- TOC entry 4850 (class 2606 OID 16762)
-- Name: RolesTranslations RolesTranslations_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RolesTranslations"
    ADD CONSTRAINT "RolesTranslations_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4862 (class 2606 OID 16919)
-- Name: StatusTranslations StatusTranslations_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."StatusTranslations"
    ADD CONSTRAINT "StatusTranslations_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4860 (class 2606 OID 16910)
-- Name: Status Status_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Status"
    ADD CONSTRAINT "Status_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4828 (class 2606 OID 16542)
-- Name: Users UserEmail; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "UserEmail" UNIQUE ("Email");


--
-- TOC entry 4830 (class 2606 OID 16540)
-- Name: Users UserName; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "UserName" UNIQUE ("UserName");


--
-- TOC entry 4806 (class 2606 OID 16396)
-- Name: Categories categories_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Categories"
    ADD CONSTRAINT categories_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4810 (class 2606 OID 16412)
-- Name: Chapters chapters_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Chapters"
    ADD CONSTRAINT chapters_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4808 (class 2606 OID 16403)
-- Name: Comics comics_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Comics"
    ADD CONSTRAINT comics_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4820 (class 2606 OID 16466)
-- Name: FanArts fanarts_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."FanArts"
    ADD CONSTRAINT fanarts_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4822 (class 2606 OID 16473)
-- Name: Orders orders_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Orders"
    ADD CONSTRAINT orders_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4826 (class 2606 OID 16480)
-- Name: Permissions permissions_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Permissions"
    ADD CONSTRAINT permissions_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4834 (class 2606 OID 16503)
-- Name: RefreshTokens refreshtokens_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RefreshTokens"
    ADD CONSTRAINT refreshtokens_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4812 (class 2606 OID 16424)
-- Name: Roles roles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Roles"
    ADD CONSTRAINT roles_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4816 (class 2606 OID 16433)
-- Name: TeamMembers teammembers_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TeamMembers"
    ADD CONSTRAINT teammembers_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4832 (class 2606 OID 16489)
-- Name: Users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT users_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4869 (class 2606 OID 16699)
-- Name: Comics ColorId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Comics"
    ADD CONSTRAINT "ColorId" FOREIGN KEY ("Color") REFERENCES public."ColorType"("Id") NOT VALID;


--
-- TOC entry 4870 (class 2606 OID 16894)
-- Name: Comics CoverId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Comics"
    ADD CONSTRAINT "CoverId" FOREIGN KEY ("Cover") REFERENCES public."CoverType"("Id") NOT VALID;


--
-- TOC entry 4886 (class 2606 OID 16763)
-- Name: CategoriesTranslations FK_Cat; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CategoriesTranslations"
    ADD CONSTRAINT "FK_Cat" FOREIGN KEY ("CategoryId") REFERENCES public."Categories"("Id") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4887 (class 2606 OID 16768)
-- Name: CategoriesTranslations FK_Cat_Language; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CategoriesTranslations"
    ADD CONSTRAINT "FK_Cat_Language" FOREIGN KEY ("LanguageId") REFERENCES public."Languages"("Id") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4890 (class 2606 OID 16822)
-- Name: ColorTypeTranslations FK_ColorTranslation_Color; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ColorTypeTranslations"
    ADD CONSTRAINT "FK_ColorTranslation_Color" FOREIGN KEY ("ColorTypeId") REFERENCES public."ColorType"("Id") ON DELETE CASCADE;


--
-- TOC entry 4891 (class 2606 OID 16827)
-- Name: ColorTypeTranslations FK_ColorTranslations_Language; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ColorTypeTranslations"
    ADD CONSTRAINT "FK_ColorTranslations_Language" FOREIGN KEY ("LanguageId") REFERENCES public."Languages"("Id") ON DELETE CASCADE;


--
-- TOC entry 4892 (class 2606 OID 16839)
-- Name: ComicsTranslations FK_ComicTranslation_Comic; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ComicsTranslations"
    ADD CONSTRAINT "FK_ComicTranslation_Comic" FOREIGN KEY ("ComicId") REFERENCES public."Comics"("Id") ON DELETE CASCADE;


--
-- TOC entry 4893 (class 2606 OID 16844)
-- Name: ComicsTranslations FK_ComicTranslation_Language; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ComicsTranslations"
    ADD CONSTRAINT "FK_ComicTranslation_Language" FOREIGN KEY ("LanguageId") REFERENCES public."Languages"("Id") ON DELETE CASCADE;


--
-- TOC entry 4884 (class 2606 OID 16810)
-- Name: ComicCategory FK_Comic_Category; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ComicCategory"
    ADD CONSTRAINT "FK_Comic_Category" FOREIGN KEY ("CategoryId") REFERENCES public."Categories"("Id") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4885 (class 2606 OID 16805)
-- Name: ComicCategory FK_Comic_Comic; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ComicCategory"
    ADD CONSTRAINT "FK_Comic_Comic" FOREIGN KEY ("ComicId") REFERENCES public."Comics"("Id") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4875 (class 2606 OID 16800)
-- Name: ComicsMembers FK_ComicsMember_Comic; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ComicsMembers"
    ADD CONSTRAINT "FK_ComicsMember_Comic" FOREIGN KEY ("ComicId") REFERENCES public."Comics"("Id") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4876 (class 2606 OID 16795)
-- Name: ComicsMembers FK_ComicsMember_Member; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."ComicsMembers"
    ADD CONSTRAINT "FK_ComicsMember_Member" FOREIGN KEY ("TeamMemberId") REFERENCES public."TeamMembers"("Id") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4894 (class 2606 OID 16899)
-- Name: CoverTypeTranslations FK_CoverTranslations_Cover; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CoverTypeTranslations"
    ADD CONSTRAINT "FK_CoverTranslations_Cover" FOREIGN KEY ("CoverTypeId") REFERENCES public."CoverType"("Id") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4895 (class 2606 OID 16875)
-- Name: CoverTypeTranslations FK_CoverTranslations_Language; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CoverTypeTranslations"
    ADD CONSTRAINT "FK_CoverTranslations_Language" FOREIGN KEY ("LanguageId") REFERENCES public."Languages"("Id") ON DELETE CASCADE;


--
-- TOC entry 4898 (class 2606 OID 16959)
-- Name: Likes FK_Like_Comic; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Likes"
    ADD CONSTRAINT "FK_Like_Comic" FOREIGN KEY ("ComicId") REFERENCES public."Comics"("Id") ON DELETE CASCADE;


--
-- TOC entry 4899 (class 2606 OID 16964)
-- Name: Likes FK_Like_User; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Likes"
    ADD CONSTRAINT "FK_Like_User" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- TOC entry 4900 (class 2606 OID 16976)
-- Name: Loves FK_Loves_Comic; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Loves"
    ADD CONSTRAINT "FK_Loves_Comic" FOREIGN KEY ("ComicId") REFERENCES public."Comics"("Id") ON DELETE CASCADE;


--
-- TOC entry 4901 (class 2606 OID 16981)
-- Name: Loves FK_Loves_User; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Loves"
    ADD CONSTRAINT "FK_Loves_User" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- TOC entry 4902 (class 2606 OID 16998)
-- Name: Pages FK_Pages_Chapter; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Pages"
    ADD CONSTRAINT "FK_Pages_Chapter" FOREIGN KEY ("ChapterId") REFERENCES public."Chapters"("Id") ON DELETE CASCADE;


--
-- TOC entry 4888 (class 2606 OID 16773)
-- Name: RolesTranslations FK_Role; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RolesTranslations"
    ADD CONSTRAINT "FK_Role" FOREIGN KEY ("RoleId") REFERENCES public."Roles"("Id") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4879 (class 2606 OID 16790)
-- Name: TeamMemberRole FK_Role; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TeamMemberRole"
    ADD CONSTRAINT "FK_Role" FOREIGN KEY ("RolesId") REFERENCES public."Roles"("Id") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4889 (class 2606 OID 16778)
-- Name: RolesTranslations FK_Role_Language; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RolesTranslations"
    ADD CONSTRAINT "FK_Role_Language" FOREIGN KEY ("LanguageId") REFERENCES public."Languages"("Id") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4896 (class 2606 OID 16920)
-- Name: StatusTranslations FK_StatusTranslation_Status; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."StatusTranslations"
    ADD CONSTRAINT "FK_StatusTranslation_Status" FOREIGN KEY ("StatusId") REFERENCES public."Status"("Id") ON DELETE CASCADE;


--
-- TOC entry 4897 (class 2606 OID 16925)
-- Name: StatusTranslations FK_StatusTranslations_Language; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."StatusTranslations"
    ADD CONSTRAINT "FK_StatusTranslations_Language" FOREIGN KEY ("LanguageId") REFERENCES public."Languages"("Id") ON DELETE CASCADE;


--
-- TOC entry 4880 (class 2606 OID 16785)
-- Name: TeamMemberRole FK_TeamMember; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."TeamMemberRole"
    ADD CONSTRAINT "FK_TeamMember" FOREIGN KEY ("TeamMemberId") REFERENCES public."TeamMembers"("Id") ON DELETE CASCADE NOT VALID;


--
-- TOC entry 4871 (class 2606 OID 16986)
-- Name: Comics StatusId; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Comics"
    ADD CONSTRAINT "StatusId" FOREIGN KEY ("Status") REFERENCES public."Status"("Id") ON DELETE SET NULL NOT VALID;


--
-- TOC entry 4882 (class 2606 OID 16609)
-- Name: CartItems fk_cartitems_cart ; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CartItems"
    ADD CONSTRAINT "fk_cartitems_cart " FOREIGN KEY ("CartId ") REFERENCES public."Cart"("Id");


--
-- TOC entry 4883 (class 2606 OID 16614)
-- Name: CartItems fk_cartitems_product ; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."CartItems"
    ADD CONSTRAINT "fk_cartitems_product " FOREIGN KEY ("ComicId") REFERENCES public."Comics"("Id");


--
-- TOC entry 4881 (class 2606 OID 16596)
-- Name: Cart fk_carts_user ; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Cart"
    ADD CONSTRAINT "fk_carts_user " FOREIGN KEY ("UserId") REFERENCES public."Users"("Id");


--
-- TOC entry 4872 (class 2606 OID 16413)
-- Name: Chapters fk_chapters_comic; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Chapters"
    ADD CONSTRAINT fk_chapters_comic FOREIGN KEY ("ComicId") REFERENCES public."Comics"("Id") ON DELETE CASCADE;


--
-- TOC entry 4873 (class 2606 OID 16942)
-- Name: Chapters fk_language; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Chapters"
    ADD CONSTRAINT fk_language FOREIGN KEY ("Original_Language") REFERENCES public."Languages"("Id") ON DELETE SET NULL NOT VALID;


--
-- TOC entry 4878 (class 2606 OID 16504)
-- Name: RefreshTokens fk_refreshtokens_user; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."RefreshTokens"
    ADD CONSTRAINT fk_refreshtokens_user FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- TOC entry 4874 (class 2606 OID 16947)
-- Name: Chapters fk_status; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Chapters"
    ADD CONSTRAINT fk_status FOREIGN KEY ("Status") REFERENCES public."Status"("Id") ON DELETE SET NULL NOT VALID;


--
-- TOC entry 4877 (class 2606 OID 16490)
-- Name: Users fk_users_permission; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT fk_users_permission FOREIGN KEY ("PermissionId") REFERENCES public."Permissions"("Id") ON DELETE SET NULL;


-- Completed on 2025-07-07 19:59:12

--
-- PostgreSQL database dump complete
--

