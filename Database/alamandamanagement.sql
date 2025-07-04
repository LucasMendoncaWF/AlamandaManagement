--
-- PostgreSQL database dump
--

-- Dumped from database version 17.5
-- Dumped by pg_dump version 17.5

-- Started on 2025-07-04 20:20:30

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
-- Name: Cart; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Cart" (
    "Id" integer NOT NULL,
    "UserId" integer NOT NULL
);


--
-- TOC entry 242 (class 1259 OID 16602)
-- Name: CartItems; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."CartItems" (
    "Id" integer NOT NULL,
    "CartId " integer NOT NULL,
    "ComicId" integer NOT NULL,
    "Quantity" integer DEFAULT 1 NOT NULL
);


--
-- TOC entry 241 (class 1259 OID 16601)
-- Name: CartItems_Id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public."CartItems_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4920 (class 0 OID 0)
-- Dependencies: 241
-- Name: CartItems_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public."CartItems_Id_seq" OWNED BY public."CartItems"."Id";


--
-- TOC entry 239 (class 1259 OID 16589)
-- Name: Cart_Id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public."Cart_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4921 (class 0 OID 0)
-- Dependencies: 239
-- Name: Cart_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public."Cart_Id_seq" OWNED BY public."Cart"."Id";


--
-- TOC entry 218 (class 1259 OID 16391)
-- Name: Categories; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Categories" (
    "Id" integer NOT NULL,
    "Name" character varying(50) NOT NULL
);


--
-- TOC entry 222 (class 1259 OID 16405)
-- Name: Chapters; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Chapters" (
    "Id" integer NOT NULL,
    "ComicId" integer NOT NULL,
    "Position" integer NOT NULL,
    "MAP" jsonb
);


--
-- TOC entry 220 (class 1259 OID 16398)
-- Name: Comics; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Comics" (
    "Id" integer NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Picture" text
);


--
-- TOC entry 227 (class 1259 OID 16442)
-- Name: ComicsMembers; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."ComicsMembers" (
    "ComicId" integer NOT NULL,
    "TeamMemberId" integer NOT NULL
);


--
-- TOC entry 229 (class 1259 OID 16459)
-- Name: FanArts; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."FanArts" (
    "Id" integer NOT NULL,
    "Social" character varying(50) NOT NULL,
    "Picture" text NOT NULL
);


--
-- TOC entry 231 (class 1259 OID 16468)
-- Name: Orders; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Orders" (
    "Id" integer NOT NULL
);


--
-- TOC entry 233 (class 1259 OID 16475)
-- Name: Permissions; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Permissions" (
    "Id" integer NOT NULL,
    "Name" character varying(30) NOT NULL
);


--
-- TOC entry 237 (class 1259 OID 16496)
-- Name: RefreshTokens; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."RefreshTokens" (
    "Id" integer NOT NULL,
    "Token" text NOT NULL,
    "Expires" date NOT NULL,
    "UserId" integer NOT NULL
);


--
-- TOC entry 224 (class 1259 OID 16419)
-- Name: Roles; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Roles" (
    "Id" integer NOT NULL,
    "Name" character varying(50) NOT NULL
);


--
-- TOC entry 238 (class 1259 OID 16510)
-- Name: TeamMemberRole; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."TeamMemberRole" (
    "TeamMemberId" integer NOT NULL,
    "RolesId" integer NOT NULL
);


--
-- TOC entry 226 (class 1259 OID 16426)
-- Name: TeamMembers; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."TeamMembers" (
    "Id" integer NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Social" character varying(50) NOT NULL,
    "Picture" text
);


--
-- TOC entry 235 (class 1259 OID 16482)
-- Name: Users; Type: TABLE; Schema: public; Owner: -
--

CREATE TABLE public."Users" (
    "Id" integer NOT NULL,
    "UserName" character varying(50) NOT NULL,
    "Email" character varying(50) NOT NULL,
    "Password" character varying(100) NOT NULL,
    "Picture" text,
    "PermissionId" integer DEFAULT 2 NOT NULL
);


--
-- TOC entry 217 (class 1259 OID 16390)
-- Name: categories_Id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public."categories_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4922 (class 0 OID 0)
-- Dependencies: 217
-- Name: categories_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public."categories_Id_seq" OWNED BY public."Categories"."Id";


--
-- TOC entry 221 (class 1259 OID 16404)
-- Name: chapters_Id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public."chapters_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4923 (class 0 OID 0)
-- Dependencies: 221
-- Name: chapters_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public."chapters_Id_seq" OWNED BY public."Chapters"."Id";


--
-- TOC entry 219 (class 1259 OID 16397)
-- Name: comics_Id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public."comics_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4924 (class 0 OID 0)
-- Dependencies: 219
-- Name: comics_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public."comics_Id_seq" OWNED BY public."Comics"."Id";


--
-- TOC entry 228 (class 1259 OID 16458)
-- Name: fanarts_Id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public."fanarts_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4925 (class 0 OID 0)
-- Dependencies: 228
-- Name: fanarts_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public."fanarts_Id_seq" OWNED BY public."FanArts"."Id";


--
-- TOC entry 230 (class 1259 OID 16467)
-- Name: orders_Id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public."orders_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4926 (class 0 OID 0)
-- Dependencies: 230
-- Name: orders_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public."orders_Id_seq" OWNED BY public."Orders"."Id";


--
-- TOC entry 232 (class 1259 OID 16474)
-- Name: permissions_Id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public."permissions_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4927 (class 0 OID 0)
-- Dependencies: 232
-- Name: permissions_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public."permissions_Id_seq" OWNED BY public."Permissions"."Id";


--
-- TOC entry 236 (class 1259 OID 16495)
-- Name: refreshtokens_Id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public."refreshtokens_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4928 (class 0 OID 0)
-- Dependencies: 236
-- Name: refreshtokens_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public."refreshtokens_Id_seq" OWNED BY public."RefreshTokens"."Id";


--
-- TOC entry 223 (class 1259 OID 16418)
-- Name: roles_Id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public."roles_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4929 (class 0 OID 0)
-- Dependencies: 223
-- Name: roles_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public."roles_Id_seq" OWNED BY public."Roles"."Id";


--
-- TOC entry 225 (class 1259 OID 16425)
-- Name: teammembers_Id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public."teammembers_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4930 (class 0 OID 0)
-- Dependencies: 225
-- Name: teammembers_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public."teammembers_Id_seq" OWNED BY public."TeamMembers"."Id";


--
-- TOC entry 234 (class 1259 OID 16481)
-- Name: users_Id_seq; Type: SEQUENCE; Schema: public; Owner: -
--

CREATE SEQUENCE public."users_Id_seq"
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


--
-- TOC entry 4931 (class 0 OID 0)
-- Dependencies: 234
-- Name: users_Id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: -
--

ALTER SEQUENCE public."users_Id_seq" OWNED BY public."Users"."Id";


--
-- TOC entry 4715 (class 2604 OID 16593)
-- Name: Cart Id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Cart" ALTER COLUMN "Id" SET DEFAULT nextval('public."Cart_Id_seq"'::regclass);


--
-- TOC entry 4716 (class 2604 OID 16605)
-- Name: CartItems Id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CartItems" ALTER COLUMN "Id" SET DEFAULT nextval('public."CartItems_Id_seq"'::regclass);


--
-- TOC entry 4704 (class 2604 OID 16394)
-- Name: Categories Id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Categories" ALTER COLUMN "Id" SET DEFAULT nextval('public."categories_Id_seq"'::regclass);


--
-- TOC entry 4706 (class 2604 OID 16408)
-- Name: Chapters Id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Chapters" ALTER COLUMN "Id" SET DEFAULT nextval('public."chapters_Id_seq"'::regclass);


--
-- TOC entry 4705 (class 2604 OID 16401)
-- Name: Comics Id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Comics" ALTER COLUMN "Id" SET DEFAULT nextval('public."comics_Id_seq"'::regclass);


--
-- TOC entry 4709 (class 2604 OID 16462)
-- Name: FanArts Id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FanArts" ALTER COLUMN "Id" SET DEFAULT nextval('public."fanarts_Id_seq"'::regclass);


--
-- TOC entry 4710 (class 2604 OID 16471)
-- Name: Orders Id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Orders" ALTER COLUMN "Id" SET DEFAULT nextval('public."orders_Id_seq"'::regclass);


--
-- TOC entry 4711 (class 2604 OID 16478)
-- Name: Permissions Id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Permissions" ALTER COLUMN "Id" SET DEFAULT nextval('public."permissions_Id_seq"'::regclass);


--
-- TOC entry 4714 (class 2604 OID 16499)
-- Name: RefreshTokens Id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."RefreshTokens" ALTER COLUMN "Id" SET DEFAULT nextval('public."refreshtokens_Id_seq"'::regclass);


--
-- TOC entry 4707 (class 2604 OID 16422)
-- Name: Roles Id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Roles" ALTER COLUMN "Id" SET DEFAULT nextval('public."roles_Id_seq"'::regclass);


--
-- TOC entry 4708 (class 2604 OID 16429)
-- Name: TeamMembers Id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."TeamMembers" ALTER COLUMN "Id" SET DEFAULT nextval('public."teammembers_Id_seq"'::regclass);


--
-- TOC entry 4712 (class 2604 OID 16485)
-- Name: Users Id; Type: DEFAULT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Users" ALTER COLUMN "Id" SET DEFAULT nextval('public."users_Id_seq"'::regclass);


--
-- TOC entry 4759 (class 2606 OID 16608)
-- Name: CartItems CartItems_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CartItems"
    ADD CONSTRAINT "CartItems_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4757 (class 2606 OID 16595)
-- Name: Cart Cart_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Cart"
    ADD CONSTRAINT "Cart_pkey" PRIMARY KEY ("Id");


--
-- TOC entry 4719 (class 2606 OID 16532)
-- Name: Categories CategoryName; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Categories"
    ADD CONSTRAINT "CategoryName" UNIQUE ("Name");


--
-- TOC entry 4723 (class 2606 OID 16534)
-- Name: Comics ComicName; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Comics"
    ADD CONSTRAINT "ComicName" UNIQUE ("Name");


--
-- TOC entry 4733 (class 2606 OID 16544)
-- Name: TeamMembers MemberSocial; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."TeamMembers"
    ADD CONSTRAINT "MemberSocial" UNIQUE ("Social");


--
-- TOC entry 4737 (class 2606 OID 16526)
-- Name: ComicsMembers PK_TeamMemberComic; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ComicsMembers"
    ADD CONSTRAINT "PK_TeamMemberComic" PRIMARY KEY ("ComicId", "TeamMemberId");


--
-- TOC entry 4755 (class 2606 OID 16528)
-- Name: TeamMemberRole PK_TeamMemberRole; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."TeamMemberRole"
    ADD CONSTRAINT "PK_TeamMemberRole" PRIMARY KEY ("TeamMemberId", "RolesId");


--
-- TOC entry 4743 (class 2606 OID 16536)
-- Name: Permissions PermissionName; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Permissions"
    ADD CONSTRAINT "PermissionName" UNIQUE ("Name");


--
-- TOC entry 4729 (class 2606 OID 16530)
-- Name: Roles RoleName; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Roles"
    ADD CONSTRAINT "RoleName" UNIQUE ("Name");


--
-- TOC entry 4747 (class 2606 OID 16542)
-- Name: Users UserEmail; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "UserEmail" UNIQUE ("Email");


--
-- TOC entry 4749 (class 2606 OID 16540)
-- Name: Users UserName; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "UserName" UNIQUE ("UserName");


--
-- TOC entry 4721 (class 2606 OID 16396)
-- Name: Categories categories_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Categories"
    ADD CONSTRAINT categories_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4727 (class 2606 OID 16412)
-- Name: Chapters chapters_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Chapters"
    ADD CONSTRAINT chapters_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4725 (class 2606 OID 16403)
-- Name: Comics comics_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Comics"
    ADD CONSTRAINT comics_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4739 (class 2606 OID 16466)
-- Name: FanArts fanarts_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."FanArts"
    ADD CONSTRAINT fanarts_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4741 (class 2606 OID 16473)
-- Name: Orders orders_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Orders"
    ADD CONSTRAINT orders_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4745 (class 2606 OID 16480)
-- Name: Permissions permissions_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Permissions"
    ADD CONSTRAINT permissions_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4753 (class 2606 OID 16503)
-- Name: RefreshTokens refreshtokens_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."RefreshTokens"
    ADD CONSTRAINT refreshtokens_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4731 (class 2606 OID 16424)
-- Name: Roles roles_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Roles"
    ADD CONSTRAINT roles_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4735 (class 2606 OID 16433)
-- Name: TeamMembers teammembers_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."TeamMembers"
    ADD CONSTRAINT teammembers_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4751 (class 2606 OID 16489)
-- Name: Users users_pkey; Type: CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT users_pkey PRIMARY KEY ("Id");


--
-- TOC entry 4765 (class 2606 OID 16520)
-- Name: TeamMemberRole FK_TeamMemberRole_Role; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."TeamMemberRole"
    ADD CONSTRAINT "FK_TeamMemberRole_Role" FOREIGN KEY ("RolesId") REFERENCES public."Roles"("Id");


--
-- TOC entry 4766 (class 2606 OID 16515)
-- Name: TeamMemberRole FK_TeamMemberRole_TeamMember; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."TeamMemberRole"
    ADD CONSTRAINT "FK_TeamMemberRole_TeamMember" FOREIGN KEY ("TeamMemberId") REFERENCES public."TeamMembers"("Id");


--
-- TOC entry 4768 (class 2606 OID 16609)
-- Name: CartItems fk_cartitems_cart ; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CartItems"
    ADD CONSTRAINT "fk_cartitems_cart " FOREIGN KEY ("CartId ") REFERENCES public."Cart"("Id");


--
-- TOC entry 4769 (class 2606 OID 16614)
-- Name: CartItems fk_cartitems_product ; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."CartItems"
    ADD CONSTRAINT "fk_cartitems_product " FOREIGN KEY ("ComicId") REFERENCES public."Comics"("Id");


--
-- TOC entry 4767 (class 2606 OID 16596)
-- Name: Cart fk_carts_user ; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Cart"
    ADD CONSTRAINT "fk_carts_user " FOREIGN KEY ("UserId") REFERENCES public."Users"("Id");


--
-- TOC entry 4760 (class 2606 OID 16413)
-- Name: Chapters fk_chapters_comic; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Chapters"
    ADD CONSTRAINT fk_chapters_comic FOREIGN KEY ("ComicId") REFERENCES public."Comics"("Id") ON DELETE CASCADE;


--
-- TOC entry 4761 (class 2606 OID 16448)
-- Name: ComicsMembers fk_comicsmembers_comic; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ComicsMembers"
    ADD CONSTRAINT fk_comicsmembers_comic FOREIGN KEY ("ComicId") REFERENCES public."Comics"("Id") ON DELETE CASCADE;


--
-- TOC entry 4762 (class 2606 OID 16453)
-- Name: ComicsMembers fk_comicsmembers_member; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."ComicsMembers"
    ADD CONSTRAINT fk_comicsmembers_member FOREIGN KEY ("TeamMemberId") REFERENCES public."TeamMembers"("Id") ON DELETE CASCADE;


--
-- TOC entry 4764 (class 2606 OID 16504)
-- Name: RefreshTokens fk_refreshtokens_user; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."RefreshTokens"
    ADD CONSTRAINT fk_refreshtokens_user FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- TOC entry 4763 (class 2606 OID 16490)
-- Name: Users fk_users_permission; Type: FK CONSTRAINT; Schema: public; Owner: -
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT fk_users_permission FOREIGN KEY ("PermissionId") REFERENCES public."Permissions"("Id") ON DELETE SET NULL;


-- Completed on 2025-07-04 20:20:30

--
-- PostgreSQL database dump complete
--

