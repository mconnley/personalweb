// Smooth scroll + active link highlighting + dark mode + year

document.addEventListener("DOMContentLoaded", () => {
  const navLinks = document.querySelectorAll(".nav-link");
  const sections = [...document.querySelectorAll("section")];
  const navToggle = document.getElementById("navToggle");
  const navLinksContainer = document.querySelector(".nav-links");
  const themeToggle = document.getElementById("themeToggle");
  const themeIcon = document.querySelector(".theme-icon");
  const yearSpan = document.getElementById("year");

  // Set year in footer
  if (yearSpan) {
    yearSpan.textContent = new Date().getFullYear();
  }

  // Smooth scroll
  navLinks.forEach((link) => {
    link.addEventListener("click", (e) => {
      e.preventDefault();
      const targetId = link.getAttribute("href");
      const target = document.querySelector(targetId);
      if (target) {
        const headerOffset = 72;
        const elementPosition = target.getBoundingClientRect().top + window.scrollY;
        const offsetPosition = elementPosition - headerOffset;

        window.scrollTo({
          top: offsetPosition,
          behavior: "smooth",
        });
      }

      // Close mobile nav
      navLinksContainer.classList.remove("open");
    });
  });

  // Highlight active section
  function handleScroll() {
    const scrollPos = window.scrollY + 80;

    let current = sections[0].id;
    sections.filter((section) => {
      return section.getAttribute("highlightmenu") == "yes";
    }).forEach((section) => {
      const top = section.offsetTop;
      const bottom = top + section.offsetHeight;
      if (scrollPos >= top && scrollPos < bottom) {
        current = section.id;
      }
    });

    navLinks.forEach((link) => {
      const targetId = link.getAttribute("href").substring(1);
      if (targetId === current) {
        link.classList.add("active");
      } else {
        link.classList.remove("active");
      }
    });

    const footerTop = document.querySelector("footer").offsetTop;
    if (window.scrollY + window.innerHeight >= footerTop) {
      navLinks.forEach((link) => {
      const targetId = link.getAttribute("href").substring(1);
      if (targetId === "contact") {
        link.classList.add("active");
      } else {
        link.classList.remove("active");
      }
    });
    }
  }

  window.addEventListener("scroll", handleScroll);
  handleScroll();

  // Mobile nav toggle
  if (navToggle && navLinksContainer) {
    navToggle.addEventListener("click", () => {
      navLinksContainer.classList.toggle("open");
    });
  }

  // Theme toggle with localStorage
  const storedTheme = localStorage.getItem("theme");
  if (storedTheme === "dark") {
    document.documentElement.setAttribute("data-theme", "dark");
    themeIcon.textContent = "🌙";
  }

  if (themeToggle) {
    themeToggle.addEventListener("click", () => {
      const currentTheme =
        document.documentElement.getAttribute("data-theme") || "light";
      const newTheme = currentTheme === "light" ? "dark" : "light";

      document.documentElement.setAttribute("data-theme", newTheme);
      localStorage.setItem("theme", newTheme);
      themeIcon.textContent = newTheme === "dark" ? "🌙" : "☀️";
    });
  }
});
